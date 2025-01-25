using System;
using System.Text;
using UnityEditor;

namespace Appegy.Union.Sample
{
    public class SolutionModifier : AssetPostprocessor
    {
        private const string ExternalProjectsFolderGuid = "{13C26F49-C8CC-48D0-81DA-7B82BB836957}";
        private const string SolutionFolderTypeGuid = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
        private const string CSharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        private const string GeneratorProjectGuid = "{D3559EE9-46DE-454D-82C2-1DB0D0FF0ADA}";
        private const string GeneratorSampleProjectGuid = "{7317372A-748F-4D5B-9086-9F82923D25CE}";
        private const string GeneratorTestsProjectGuid = "{DCA34C60-E33A-4BC4-865F-626306023A01}";

        public static string OnGeneratedSlnSolution(string path, string content)
        {
            if (!path.EndsWith(".sln"))
                return content;

            var sb = new StringBuilder(content);

            AddFolder(sb, "ExternalProjects", ExternalProjectsFolderGuid);

            AddProjectToFolder(sb, "Appegy.Union.Generator", GeneratorProjectGuid, "..\\\\Appegy.Union.Generator~\\\\Appegy.Union.Generator\\\\Appegy.Union.Generator.csproj");
            AddProjectToFolder(sb, "Appegy.Union.Generator.Sample", GeneratorSampleProjectGuid, "..\\\\Appegy.Union.Generator~\\\\Appegy.Union.Generator.Sample\\\\Appegy.Union.Generator.Sample.csproj");
            AddProjectToFolder(sb, "Appegy.Union.Generator.Tests", GeneratorTestsProjectGuid, "..\\\\Appegy.Union.Generator~\\\\Appegy.Union.Generator.Tests\\\\Appegy.Union.Generator.Tests.csproj");

            AddProjectConfiguration(sb, GeneratorProjectGuid);
            AddProjectConfiguration(sb, GeneratorSampleProjectGuid);
            AddProjectConfiguration(sb, GeneratorTestsProjectGuid);

            AddNestedProjectsSection(sb, ExternalProjectsFolderGuid, GeneratorProjectGuid, GeneratorSampleProjectGuid, GeneratorTestsProjectGuid);

            return sb.ToString();
        }

        private static void AddFolder(StringBuilder sb, string folderName, string folderGuid)
        {
            sb.Insert(sb.ToString().IndexOf("Global", StringComparison.Ordinal),
                $"Project(\"{SolutionFolderTypeGuid}\") = \"{folderName}\", \"{folderName}\", \"{folderGuid}\"\n" +
                "EndProject\n");
        }

        private static void AddProjectToFolder(StringBuilder sb, string projectName, string projectGuid, string projectPath)
        {
            sb.Insert(sb.ToString().IndexOf("Global", StringComparison.Ordinal),
                $"Project(\"{CSharpProjectTypeGuid}\") = \"{projectName}\", \"{projectPath}\", \"{projectGuid}\"\n" +
                "EndProject\n");
        }

        private static void AddProjectConfiguration(StringBuilder sb, string projectGuid)
        {
            var projectConfigurationsIndex = sb.ToString().IndexOf("EndGlobalSection", sb.ToString().IndexOf("GlobalSection(ProjectConfigurationPlatforms)", StringComparison.Ordinal), StringComparison.Ordinal) - 1;
            sb.Insert(projectConfigurationsIndex,
                $"\t\t{projectGuid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\n" +
                $"\t\t{projectGuid}.Debug|Any CPU.Build.0 = Debug|Any CPU\n");
        }

        private static void AddNestedProjectsSection(StringBuilder sb, string folderGuid, params string[] projectGuids)
        {
            var endGlobalIndex = sb.ToString().LastIndexOf("EndGlobal", StringComparison.Ordinal);
            var nestedProjectsSection = new StringBuilder("\n\tGlobalSection(NestedProjects) = preSolution\n");

            foreach (var projectGuid in projectGuids)
            {
                nestedProjectsSection.AppendLine($"\t\t{projectGuid} = {folderGuid}");
            }

            nestedProjectsSection.AppendLine("\tEndGlobalSection\n");
            sb.Insert(endGlobalIndex, nestedProjectsSection.ToString());
        }
    }
}