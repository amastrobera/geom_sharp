param([parameter()] [validateset("local")] [string] $target) 


Write-Host "===== deployment ======"

$repo_local = "local"
$repo_nuget = "nuget.org"
$lib_name = "GeomSharp"
$lib_nuspec = "GeomSharp.nuspec"

$target_repo = If ($target -ieq $repo_local) { $repo_local } Else { $repo_nuget }
Write-Host "   > selected repo: $target_repo"


Write-Host "   > available sources"
$repos = $(nuget sources list)
for ($i=1; $i -lt $repos.Count; $i++) { 
    if ($i % 2 -eq 1) {
        Write-Host "   "  $repos[$i]
    }
}
Write-Host "   > selected repo: $target_repo"

$proj_dir = $(Join-Path $pwd $lib_name)
$pkg_spec_path = $(Join-Path $proj_dir $lib_nuspec)
Write-Host "pkg_spec_path = $pkg_spec_path"

$pkg_out_dir = $(Join-Path $proj_dir nupkg)
Write-Host "pkg_out_dir = $pkg_out_dir"

[xml]$xmlElm = Get-Content -Path $pkg_spec_path 
$xml_id = $xmlElm.package.metadata.id
Write-Host "id = $xml_id"
$xml_version = $xmlElm.package.metadata.version
Write-Host "version = $xml_version"

msbuild $proj_dir -t:build -property:Configuration=Release -restore

nuget pack $pkg_spec_path -Build -Symbols -Properties Configuration=Release -OutputDirectory $pkg_out_dir -IncludeReferencedProjects

$pkg_out_sym = $(Join-Path $pkg_out_dir "$xml_id.$xml_version.symbols.nupkg")
Write-Host "package w/symbols = $pkg_out_sym"

If ($target -ieq $repo_local) {
    Write-Host "pushing to local ..."

    # delete anything previously deployed 
    $local_paths = $($(nuget locals global-packages -list) -split "global-packages:")
    if ($local_paths.Count -lt 2) { 
        Write-Error "missing local path, create one before deploying to $repo_local"
        Exit 1
    }
    $main_local_path = $local_paths[1].Trim()
    if (-Not (Test-Path $main_local_path)) {
        Write-Error "the $repo_local path '$main_local_path' does not exist, cannot delete it"
        Exit 2
    }
    $lib_local_path = Join-Path $main_local_path $lib_name
    if (-Not (Test-Path $lib_local_path)) {
        Write-Error "the $repo_local path '$lib_local_path' does not exist, cannot delete it"
        Exit 3
    }
    $lib_local_path_version = Join-Path $lib_local_path $xml_version
    if (Test-Path $lib_local_path_version) {
        # it exists, then delete stuff 
        Write-Host "deleting pre-existing version ..."
        Remove-Item -Force -Recurse $lib_local_path_version
    }
}
    nuget push $pkg_out_sym -Source $target_repo -SymbolSource $target_repo -SkipDuplicate
