param($installPath, $toolsPath, $package, $project)

$DTE.ItemOperations.Navigate("http://sadconsole.com/docs/nuget-starter-monogame.html")

$project.ProjectItems["IBM8x16.png"].Properties["CopyToOutputDirectory"].Value = 2
$project.ProjectItems["Cheepicus_12x12.png"].Properties["CopyToOutputDirectory"].Value = 2
$project.ProjectItems["Yayo_c64.png"].Properties["CopyToOutputDirectory"].Value = 2
$project.ProjectItems["IBM.font"].Properties["CopyToOutputDirectory"].Value = 2
$project.ProjectItems["Cheepicus12.font"].Properties["CopyToOutputDirectory"].Value = 2
$project.ProjectItems["C64.font"].Properties["CopyToOutputDirectory"].Value = 2

