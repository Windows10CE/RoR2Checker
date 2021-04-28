﻿using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Mono.Cecil;
using Mono.Cecil.Cil;
using RoR2Checker.Models.Thunderstore;

namespace RoR2Checker.Modules
{
    public class Check : ModuleBase<SocketCommandContext>
    {
        [Command("check")]
        [Alias("c")]
        [RequireOwner]
        public async Task CheckPackageAsync([Remainder] string info)
        {
            using var typing = Context.Channel.EnterTypingState();

            var nameParts = info.Split(' ', '-', '/');
            if (nameParts.Length < 2)
            {
                await ReplyAsync("Invalid package name");
                return;
            }

            var pkg = await PackageInfo.FromAuthorAndNameAsync(nameParts[0], nameParts[1]);

            using (var http = new HttpClient())
            {
                var response = await http.SendAsync(new HttpRequestMessage()
                {
                    RequestUri = new Uri(pkg.latest.download_url)
                });

                if (!response.IsSuccessStatusCode)
                {
                    await ReplyAsync("Error while downloading package");
                    return;
                }

                using (var zip = new ZipArchive(await response.Content.ReadAsStreamAsync()))
                {
                    var failures = new Fails();

                    var dllsToCheck = new List<AssemblyDefinition>();

                    var reader = new ReaderParameters();

                    var resolver = new DefaultAssemblyResolver();
                    resolver.AddSearchDirectory("ReferenceAssemblies");
                    resolver.ResolveFailure += (sender, asmName) =>
                    {
                        failures.Assemblies.Add(asmName.FullName);
                        return AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(asmName.Name, asmName.Version), "<Module>", ModuleKind.Dll);
                    };

                    reader.AssemblyResolver = resolver;

                    foreach (var asmEntry in zip.Entries.Where(x => x.Name.ToLower().EndsWith(".dll")))
                    {
                        var tempName = Path.Combine("Temp", Guid.NewGuid().ToString());
                        
                        asmEntry.ExtractToFile(tempName);

                        dllsToCheck.Add(AssemblyDefinition.ReadAssembly(tempName, reader));
                    }

                    bool anyFailed = false;
                    StringBuilder replyBuilder = new StringBuilder();

                    foreach (var asm in dllsToCheck)
                    {
                        CheckDLL(asm, ref failures);
                        anyFailed |= failures.Any;

                        if (failures.Any)
                            replyBuilder.AppendLine($"{asm.Name.Name} failures:");
                        if (failures.Assemblies.Any())
                        {
                            replyBuilder.AppendLine("**Missing Assemblies:**");
                            foreach (var missing in failures.Assemblies)
                                replyBuilder.AppendLine(missing);
                        }
                        if (failures.Types.Any())
                        {
                            replyBuilder.AppendLine("**Missing Types:**");
                            foreach (var missing in failures.Types)
                                replyBuilder.AppendLine(missing);
                        }
                        if (failures.Methods.Any())
                        {
                            replyBuilder.AppendLine("**Missing Methods:**");
                            foreach (var missing in failures.Methods)
                                replyBuilder.AppendLine(missing);
                        }
                        if (failures.Fields.Any())
                        {
                            replyBuilder.AppendLine("**Missing Fields:**");
                            foreach (var missing in failures.Fields)
                                replyBuilder.AppendLine(missing);
                        }
                    }

                    await ReplyAsync(anyFailed ? replyBuilder.ToString().Replace("`", "\\`") : $"{pkg.full_name} Passed");
                }
            }
        }

        private Fails CheckDLL(AssemblyDefinition asm, ref Fails failures)
        {
            foreach (var typeRef in asm.MainModule.GetTypeReferences())
            {
                if (typeRef.Resolve() == null)
                {
                    failures.Types.Add(typeRef.FullName);
                    failures.Any = true;
                }
            }
            foreach (var method in asm.MainModule.Types.SelectMany(x => x.Methods))
            {
                if (!method.HasBody) continue;

                foreach (var inst in method.Body.Instructions)
                {
                    if (inst.OpCode == OpCodes.Call || inst.OpCode == OpCodes.Callvirt)
                    {
                        var methodRef = (MethodReference)inst.Operand;
                        if (methodRef.Resolve() == null && !failures.Methods.Contains(methodRef.FullName))
                        {
                            failures.Methods.Add(methodRef.FullName);
                            failures.Any = true;
                        }
                    }
                    if (inst.OpCode == OpCodes.Ldfld || inst.OpCode == OpCodes.Ldflda || inst.OpCode == OpCodes.Ldsfld || inst.OpCode == OpCodes.Ldsflda)
                    {
                        var fieldRef = (FieldReference)inst.Operand;
                        if (fieldRef.Resolve() == null && !failures.Fields.Contains(fieldRef.FullName))
                        {
                            failures.Fields.Add(fieldRef.FullName);
                            failures.Any = true;
                        }
                    }
                }
            }
            return failures;
        }

        private class Fails
        {
            public bool Any = false;
            public List<string> Assemblies = new List<string>();
            public List<string> Types = new List<string>();
            public List<string> Fields = new List<string>();
            public List<string> Methods = new List<string>();
        }
    }
}