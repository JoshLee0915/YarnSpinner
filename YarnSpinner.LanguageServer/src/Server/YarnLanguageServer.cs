﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Google.Protobuf;

namespace YarnLanguageServer
{
    public class YarnLanguageServer
    {

        public static ILanguageServer server;

        private static async Task Main(string[] args)
        {
            if (args.Contains("--waitForDebugger"))
            {
                while (!Debugger.IsAttached) { await Task.Delay(100).ConfigureAwait(false); }
            }

            server = await LanguageServer.From(
                options => ConfigureOptions(options)
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
            ).ConfigureAwait(false);

            await server.WaitForExit.ConfigureAwait(false);
        }

        public static LanguageServerOptions ConfigureOptions(LanguageServerOptions options)
        {
            var workspace = new Workspace();

            options
                .WithServices(services => services.AddSingleton(workspace))
                .WithHandler<Handlers.TextDocumentHandler>()
                .WithHandler<Handlers.DocumentSymbolHandler>()
                .WithHandler<Handlers.WorkspaceSymbolHandler>()
                .WithHandler<Handlers.SemanticTokensHandler>()
                .WithHandler<Handlers.DefinitionHandler>()
                .WithHandler<Handlers.CodeLensHandler>()
                .WithHandler<Handlers.ReferencesHandler>()
                .WithHandler<Handlers.CompletionHandler>()
                .WithHandler<Handlers.SignatureHelpHandler>()
                .WithHandler<Handlers.HoverHandler>()
                .WithHandler<Handlers.ConfigurationHandler>()
                .WithHandler<Handlers.CodeActionHandler>()
                .WithHandler<Handlers.RenameHandler>()
                .WithHandler<Handlers.FileOperationsHandler>()
                .OnInitialize(async (server, request, token) =>
                {
                    try {
                        workspace.Root = request.RootPath;

                        server.Log("Server initialize.");

                        // avoid re-initializing if possible by getting config settings in early
                        if (request.InitializationOptions != null)
                        {
                            workspace.Configuration.Initialize(request.InitializationOptions as Newtonsoft.Json.Linq.JArray);
                        }
                        
                        workspace.Initialize(server);
                        await Task.CompletedTask.ConfigureAwait(false);
                    } catch (Exception e) {
                        server.Window.ShowError($"Yarn Spinner language server failed to start: {e}");
                        await Task.FromException(e).ConfigureAwait(false);
                    }
                })
                .OnInitialized(async (server, request, response, token) =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);
                })
                .OnStarted(async (server, token) =>
                {
                    await Task.CompletedTask.ConfigureAwait(false);
                })

                ;

            // Register 'List Nodes' command
            options.OnExecuteCommand<Container<NodeInfo>>(
                (commandParams) => ListNodesInDocumentAsync(workspace, commandParams),
                (_, _) => new ExecuteCommandRegistrationOptions
                {
                    Commands = new[] { Commands.ListNodes },
                }
            );

            // Register 'Add Nodes' command
            options.OnExecuteCommand<TextDocumentEdit>(
                (commandParams) => AddNodeToDocumentAsync(workspace, commandParams),
                (_, _) => new ExecuteCommandRegistrationOptions
                {
                    Commands = new[] { Commands.AddNode },
                }
            );

            // Register 'Remove Node' command
            options.OnExecuteCommand<TextDocumentEdit>(
                (commandParams) => RemoveNodeFromDocumentAsync(workspace, commandParams),
                (_, _) => new ExecuteCommandRegistrationOptions
                {
                    Commands = new[] { Commands.RemoveNode },
                }
            );

            // Register 'Update Header' command
            options.OnExecuteCommand<TextDocumentEdit>(
                (commandParams) => UpdateNodeHeaderAsync(workspace, commandParams),
                (_, _) => new ExecuteCommandRegistrationOptions
                {
                    Commands = new[] { Commands.UpdateNodeHeader },
                }
            );

            // Register 'Compile' command
            options.OnExecuteCommand<CompilerOutput>(
                (commandParams) => CompileWorkspace(workspace, commandParams),
                (_, _) => new ExecuteCommandRegistrationOptions
                {
                    Commands = new[] { Commands.Compile },
                }
            );

            // Register 'lineblock' command
            options.OnExecuteCommand<VOStringExport>(
                (commandParams) => BlockExtraction(workspace, commandParams), (_,_) => new ExecuteCommandRegistrationOptions
                {
                    Commands = new[] { Commands.Extract },
                }
            );

            // register graph dialogue command
            options.OnExecuteCommand<string>(
                (commandParams) => GenerateDialogueGraph(workspace, commandParams), (_,_) => new ExecuteCommandRegistrationOptions
                {
                    Commands = new[] { Commands.Graph },
                }
            );

            return options;
        }

        private static Task<TextDocumentEdit> AddNodeToDocumentAsync(Workspace workspace, ExecuteCommandParams<TextDocumentEdit> commandParams)
        {
            var yarnDocumentUriString = commandParams.Arguments[0].ToString();

            var headers = new Dictionary<string, string>();

            if (commandParams.Arguments.Count >= 2) {
                var headerObject = commandParams.Arguments[1] as JObject;

                foreach (var property in headerObject) {
                    headers.Add(property.Key, property.Value.ToString());
                }
            }

            Uri yarnDocumentUri = new (yarnDocumentUriString);

            if (workspace.YarnFiles.TryGetValue(yarnDocumentUri, out var yarnFile) == false)
            {
                // Try and add this file to the workspace
                yarnFile = workspace.OpenFile(yarnDocumentUri);
                if (yarnFile == null)
                {
                    // Failed to open it. Return no change.
                    return Task.FromResult(new TextDocumentEdit
                    {
                        TextDocument = new OptionalVersionedTextDocumentIdentifier
                        {
                            Uri = yarnDocumentUri,
                        },
                        Edits = new List<TextEdit>(),
                    });
                }
            }

            // Work out the edit needed to add a node.

            // Figure out the name of the new node.
            var allNodeTitles = workspace.YarnFiles.Values.SelectMany(yf => yf.NodeInfos).Select(n => n.Title);

            var candidateCount = 0;
            var candidateName = "Node";

            while (allNodeTitles.Contains(candidateName))
            {
                candidateCount += 1;
                candidateName = $"Node{candidateCount}";
            }

            var newNodeText = new System.Text.StringBuilder()
                .AppendLine($"title: {candidateName}");
           
            // Add the headers
            foreach (var h in headers) {
                newNodeText.AppendLine($"{h.Key}: {h.Value}");
            }

            newNodeText
                .AppendLine("---")
                .AppendLine()
                .AppendLine("===");

            Position position;

            // First, are there any nodes at all?
            if (yarnFile.NodeInfos.Count == 0)
            {
                // No nodes. Add one at the start.
                position = new Position(0, 0);
            }
            else
            {
                var lastLineIsEmpty = yarnFile.Text.EndsWith('\n');

                int lastLineIndex = yarnFile.LineCount - 1;

                if (lastLineIsEmpty)
                {
                    // The final line ends with a newline. Insert the node
                    // there.
                    position = new Position(lastLineIndex, 0);
                }
                else
                {
                    // The final line does not end with a newline. Insert a
                    // newline at the end of the last line, followed by the new
                    // text.
                    var endOfLastLine = yarnFile.GetLineLength(lastLineIndex);
                    newNodeText.Insert(0, Environment.NewLine);
                    position = new Position(lastLineIndex, endOfLastLine);
                }
            }

            // Return the edit that adds this node
            return Task.FromResult(new TextDocumentEdit
            {
                TextDocument = new OptionalVersionedTextDocumentIdentifier
                {
                    Uri = yarnDocumentUri,
                },
                Edits = new[] {
                    new TextEdit {
                        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(position, position),
                        NewText = newNodeText.ToString(),
                    },
                },
            });
        }

        private static Task<TextDocumentEdit> RemoveNodeFromDocumentAsync(Workspace workspace, ExecuteCommandParams<TextDocumentEdit> commandParams)
        {
            var yarnDocumentUriString = commandParams.Arguments[0].ToString();

            var nodeTitle = commandParams.Arguments[1].ToString();

            Uri yarnDocumentUri = new (yarnDocumentUriString);

            TextDocumentEdit emptyResult = new TextDocumentEdit
            {
                TextDocument = new OptionalVersionedTextDocumentIdentifier
                {
                    Uri = yarnDocumentUri,
                },
                Edits = new List<TextEdit>(),
            };

            if (workspace.YarnFiles.TryGetValue(yarnDocumentUri, out var yarnFile) == false)
            {
                // Try and add this file to the workspace
                yarnFile = workspace.OpenFile(yarnDocumentUri);
                if (yarnFile == null)
                {
                    workspace.LanguageServer.Window.ShowMessage(new ShowMessageParams
                    {
                        Message = $"Can't remove node: failed to open file ${yarnDocumentUri}",
                        Type = MessageType.Error,
                    });

                    // Failed to open it. Return no change.
                    return Task.FromResult(emptyResult);
                }
            }

            // First: does this file contain a node with this title?
            var nodes = yarnFile.NodeInfos.Where(n => n.Title == nodeTitle);

            if (nodes.Count() != 1) {
                // We need precisely 1 node to remove.
                var multipleNodesMessage = $"multiple nodes named {nodeTitle} exist in this file";
                var noNodeMessage = $"no node named {nodeTitle} exists in this file";
                workspace.LanguageServer.Window.ShowMessage(new ShowMessageParams
                {
                    Message = $"Can't remove node: {(nodes.Any() ? multipleNodesMessage : noNodeMessage)}. Modify the source code directly.",
                    Type = MessageType.Error,
                });
                return Task.FromResult(emptyResult);
            }

            var node = nodes.Single();

            // Work out the edit needed to remove the node.
            var deletionStart = new Position(node.HeaderStartLine, 0);

            // Stop deleting at the start of the line after the end-of-body
            // delimiter (which is 2 lines down from the final line of body
            // text)
            var deletionEnd = new Position(node.BodyEndLine + 2, 0);

            // Return the edit that removes this node
            return Task.FromResult(new TextDocumentEdit
            {
                TextDocument = new OptionalVersionedTextDocumentIdentifier
                {
                    Uri = yarnDocumentUri,
                },
                Edits = new[] {
                    new TextEdit {
                        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(deletionStart, deletionEnd),
                        NewText = string.Empty,
                    },
                },
            });
        }

        private static Task<TextDocumentEdit> UpdateNodeHeaderAsync(Workspace workspace, ExecuteCommandParams<TextDocumentEdit> commandParams)
        {
            var yarnDocumentUriString = commandParams.Arguments[0].ToString();

            var nodeTitle = commandParams.Arguments[1].ToString();

            var headerKey = commandParams.Arguments[2].ToString();

            var headerValue = commandParams.Arguments[3].ToString();

            Uri yarnDocumentUri = new (yarnDocumentUriString);

            TextDocumentEdit emptyResult = new TextDocumentEdit
            {
                TextDocument = new OptionalVersionedTextDocumentIdentifier
                {
                    Uri = yarnDocumentUri,
                },
                Edits = new List<TextEdit>(),
            };

            if (workspace.YarnFiles.TryGetValue(yarnDocumentUri, out var yarnFile) == false)
            {
                // Try and add this file to the workspace
                yarnFile = workspace.OpenFile(yarnDocumentUri);
                if (yarnFile == null)
                {
                    // Failed to open it. Return no change.
                    return Task.FromResult(emptyResult);
                }
            }

            // Does this file contain a node with this title?
            var nodes = yarnFile.NodeInfos.Where(n => n.Title == nodeTitle);

            if (nodes.Count() != 1) {
                // We need precisely 1 node to modify.
                var multipleNodesMessage = $"multiple nodes named {nodeTitle} exist in this file";
                var noNodeMessage = $"no node named {nodeTitle} exists in this file";
                workspace.LanguageServer.Window.ShowMessage(new ShowMessageParams
                {
                    Message = $"Can't update header node: {(nodes.Any() ? multipleNodesMessage : noNodeMessage)}. Modify the source code directly.",
                    Type = MessageType.Error,
                });
                return Task.FromResult(emptyResult);
            }

            var node = nodes.Single();

            // Does this node contain a header with this title?
            var existingHeader = node.Headers.Find(h => h.Key == headerKey);

            var headerText = $"{headerKey}: {headerValue}";

            Position startPosition;
            Position endPosition;

            if (existingHeader != null) {
                // Create an edit to replace it
                var line = existingHeader.KeyToken.Line - 1;
                startPosition = new Position(line, 0);
                endPosition = new Position(line, yarnFile.GetLineLength(line));
            } else {
                // Create an edit to insert it immediately before the body start
                // delimiter
                var line = node.BodyStartLine - 1;
                startPosition = new Position(line, 0);
                endPosition = new Position(line, 0);
                
                // Add a newline so that the delimiter stays on its own line
                headerText += Environment.NewLine;
            }

            // Return the edit that creates or updates this header
            return Task.FromResult(new TextDocumentEdit
            {
                TextDocument = new OptionalVersionedTextDocumentIdentifier
                {
                    Uri = yarnDocumentUri,
                },
                Edits = new[] {
                    new TextEdit {
                        Range = new OmniSharp.Extensions.LanguageServer.Protocol.Models.Range(startPosition, endPosition),
                        NewText = headerText,
                    },
                },
            });
        }

        private static Task<Container<NodeInfo>> ListNodesInDocumentAsync(Workspace workspace, ExecuteCommandParams<Container<NodeInfo>> commandParams)
        {
            var result = new List<NodeInfo>();

            var yarnDocumentUriString = commandParams.Arguments[0].ToString();

            Uri yarnDocumentUri = new (yarnDocumentUriString);

            if (workspace.YarnFiles.TryGetValue(yarnDocumentUri, out var yarnFile))
            {
                result = yarnFile.NodeInfos.ToList();
            }

            return Task.FromResult<Container<NodeInfo>>(result);
        }

        private static Task<CompilerOutput> CompileWorkspace(Workspace workspace, ExecuteCommandParams<CompilerOutput> commandParams)
        {
            var job = new Yarn.Compiler.CompilationJob
            {
                Files = workspace.YarnFiles.Select(pair =>
                {
                    var uri = pair.Key;
                    var file = pair.Value;

                    return new Yarn.Compiler.CompilationJob.File
                    {
                        FileName = uri.ToString(),
                        Source = file.Text,
                    };
                }),
                CompilationType = Yarn.Compiler.CompilationJob.Type.FullCompilation,
            };

            var result = Yarn.Compiler.Compiler.Compile(job);
            
            // should check in here if it worked I suppose...
            var e = result.Diagnostics.Where(d => d.Severity == Yarn.Compiler.Diagnostic.DiagnosticSeverity.Error).Select(d => d.Message).ToArray();
            workspace.PublishSpecificDiagnostics(result.Diagnostics);

            var strings = new Dictionary<string, string>();
            foreach (var line in result.StringTable)
            {
                strings[line.Key] = line.Value.text;
            }

            var output = new CompilerOutput
            {
                Data = result.Program?.ToByteArray(),
                StringTable = strings,
                Errors = e,
            };
            return Task.FromResult<CompilerOutput>(output);
        }

        private static Task<string> GenerateDialogueGraph(Workspace workspace, ExecuteCommandParams<string> commandParams)
        {
            // alright so first we get the text of every file
            var fileText = workspace.YarnFiles.Select(pair => { return pair.Value.Text; }).ToArray();

            // then we give that to the util that generates the runs
            var graph = Yarn.Compiler.Utility.DetermineNodeConnections(fileText);

            // then we build up the dot/mermaid file (copy from ysc)
            string graphString;

            // I hate this
            var format = commandParams.Arguments[0].ToString();
            var clustering = commandParams.Arguments[1].ToObject<Boolean>();

            if (format.Equals("dot"))
            {
                graphString = DrawDot(graph, clustering);
            }
            else
            {
                graphString = DrawMermaid(graph, clustering);
            }
            
            // then we send that back over
            return Task.FromResult(graphString);
        }

        // copied from YSC
        private static string DrawMermaid(List<List<Yarn.Compiler.GraphingNode>> graph, bool clustering)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("flowchart TB");

            int i = 0;
            foreach (var cluster in graph)
            {
                if (cluster.Count == 0)
                {
                    continue;
                }

                if (clustering)
                {
                    sb.AppendLine($"\tsubgraph a{i}");
                }
                foreach (var node in cluster)
                {
                    foreach (var jump in node.jumps)
                    {
                        sb.AppendLine($"\t{node.node}-->{jump}");
                    }
                }
                if (clustering)
                {
                    sb.AppendLine("\tend");
                }
                i++;
            }
            return sb.ToString();
        }
        private static string DrawDot(List<List<Yarn.Compiler.GraphingNode>> graph, bool clustering)
        {
            // using three individual builders is a bit lazy but it means I can turn stuff on and off as needed
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Text.StringBuilder links = new System.Text.StringBuilder();
            System.Text.StringBuilder sub = new System.Text.StringBuilder();
            sb.AppendLine("digraph dialogue {");

            if (clustering)
            {
                int i = 0;
                foreach (var cluster in graph)
                {
                    if (cluster.Count == 0)
                    {
                        continue;
                    }
                    
                    // they need to be named clusterSomething to be clustered
                    sub.AppendLine($"\tsubgraph cluster{i}{{");
                    sub.Append("\t\t");
                    foreach (var node in cluster)
                    {
                        sub.Append($"{node.node} ");
                    }
                    sub.AppendLine(";");
                    sub.AppendLine("\t}");
                    i++;
                }
            }

            foreach (var cluster in graph)
            {
                foreach (var connection in cluster)
                {
                    if (connection.hasPositionalInformation)
                    {
                        sb.AppendLine($"\t{connection.node} [");
                        sb.AppendLine($"\t\tpos = \"{connection.position.x},{connection.position.y}\"");
                        sb.AppendLine("\t]");
                    }

                    foreach (var link in connection.jumps)
                    {
                        links.AppendLine($"\t{connection.node} -> {link};");
                    }
                }
            }   

            sb.Append(links);
            sb.Append(sub);

            sb.AppendLine("}");
            return sb.ToString();
        }


        private static Task<VOStringExport> BlockExtraction(Workspace workspace, ExecuteCommandParams<VOStringExport> commandParams)
        {
            // compiling the whole workspace so we can get access to the program to make sure it works
            var job = new Yarn.Compiler.CompilationJob
            {
                Files = workspace.YarnFiles.Select(pair =>
                {
                    var uri = pair.Key;
                    var file = pair.Value;

                    return new Yarn.Compiler.CompilationJob.File
                    {
                        FileName = uri.ToString(),
                        Source = file.Text,
                    };
                }),
                CompilationType = Yarn.Compiler.CompilationJob.Type.FullCompilation,
            };

            var result = Yarn.Compiler.Compiler.Compile(job);

            byte[] fileData = {};
            var e = result.Diagnostics.Where(d => d.Severity == Yarn.Compiler.Diagnostic.DiagnosticSeverity.Error).Select(d => d.Message).ToArray();
            if (e.Length == 0)
            {
                // we have no errors so we can run through the nodes and build up our blocks of lines
                var lineBlocks = Yarn.Compiler.Utility.ExtractStringBlocks(result.Program.Nodes.Values).Select(bs => bs.ToArray()).ToArray();

                // I hate this
                var format = commandParams.Arguments[0].ToString();
                var columns = commandParams.Arguments[1].ToObject<string[]>();
                var defaultName = commandParams.Arguments[2].ToString();
                var useCharacters = commandParams.Arguments[3].ToObject<Boolean>();

                fileData = StringExtractor.ExportStrings(lineBlocks, result.StringTable, columns, format, defaultName, useCharacters);
            }

            var output = new VOStringExport
            {
                File = fileData,
                Errors = e,
            };
            return Task.FromResult(output);
        }
    }
}
