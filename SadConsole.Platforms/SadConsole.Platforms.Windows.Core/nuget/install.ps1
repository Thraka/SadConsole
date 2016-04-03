param($installPath, $toolsPath, $package, $project)

$DTE.ItemOperations.Navigate("https://github.com/Thraka/SadConsole/wiki/NuGet-Starter")

$project.ProjectItems["Fonts"].ProjectItems["Cheepicus_12x12.png"].Properties["CopyToOutputDirectory"].Value = 2
$project.ProjectItems["Fonts"].ProjectItems["IBM8x16.png"].Properties["CopyToOutputDirectory"].Value = 2
$project.ProjectItems["Fonts"].ProjectItems["IBM.font"].Properties["CopyToOutputDirectory"].Value = 2
$project.ProjectItems["Fonts"].ProjectItems["Cheepicus12.font"].Properties["CopyToOutputDirectory"].Value = 2