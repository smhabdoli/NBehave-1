param($installPath, $toolsPath, $package, $project)
& "$toolsPath\net40\VsPlugin\NBehave.VS2012.Plugin.vsix"
Write-Host "You need to restart Visual Studio 2012 for the plugin to be loaded." -BackgroundColor Black -ForegroundColor Yellow

$file = $project.ProjectItems.Item("Vs2012Plugin.feature")
$file.Open()
$project.DTE.ItemOperations.OpenFile($file.Document.FullName, [EnvDTE.Constants]::vsDocumentKindText)
