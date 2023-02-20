param([parameter()] [validateset("local")] [string] $target) 


Write-Host "===== deployment ======"

$target_repo = If ($target -ieq "local") { "local" } Else {"nuget.org"}
Write-Host "   > selected repo: $target_repo"


Write-Host "   > available sources"
$repos = $(nuget sources list)
for($i=1; $i -lt $repos.Count; $i++){ 
    if ($i % 2 -eq 1) {
        Write-Host "   "  $repos[$i]
    }
}
Write-Host "   > selected repo: $target_repo"

$proj_dir = $(Join-Path $pwd GeomSharp)
$pkg_spec_path = $(Join-Path $proj_dir GeomSharp.nuspec)
Write-Host "pkg_spec_path = $pkg_spec_path"

$pkg_out_dir = $(Join-Path $proj_dir nupkg)
Write-Host "pkg_out_dir = $pkg_out_dir"

[xml]$xmlElm = Get-Content -Path $pkg_spec_path 
$xml_id = $xmlElm.package.metadata.id
Write-Host "id = $xml_id"
$xml_version = $xmlElm.package.metadata.version
Write-Host "version = $xml_version"

nuget pack $pkg_spec_path -Build -Symbols -Properties Configuration=Release -OutputDirectory $pkg_out_dir -IncludeReferencedProjects 

# $pkg_out_file = $(Join-Path $pkg_out_dir "$xml_id.$xml_version.nupkg")
# Write-Host "package = $pkg_out_file"
#nuget push $pkg_out_file -Source $target_repo -SkipDuplicate

$pkg_out_sym = $(Join-Path $pkg_out_dir "$xml_id.$xml_version.symbols.nupkg")
Write-Host "package w/symbols = $pkg_out_sym"

nuget push $pkg_out_sym -Source $target_repo -SymbolSource $target_repo -SkipDuplicate
