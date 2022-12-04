

Write-Host "===== local deployment ======"

$target_repo = "local"

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
$xml_version = $xmlElm.package.metadata.version
Write-Host "version = $xml_version"

nuget pack $pkg_spec_path -Build -Symbols -Properties Configuration=Release -OutputDirectory $pkg_out_dir -IncludeReferencedProjects
$pkg_out_file = $(ls $(Join-Path $pkg_out_dir *$xml_version.nupkg))[0].FullName
Write-Host "package = $(ls $pkg_out_file)"

$pkg_out_sym = $(ls $(Join-Path $pkg_out_dir *$xml_version.symbols.nupkg))[0].FullName
Write-Host "symbols = $(ls $pkg_out_sym)"

#nuget push $pkg_out_file -Source $target_repo -SkipDuplicate
nuget push $pkg_out_sym -Source $target_repo -SymbolSource $target_repo -SkipDuplicate