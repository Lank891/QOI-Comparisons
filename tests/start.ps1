$dir=$args[0]
$out=$args[1]

$args = ""

$children = Get-ChildItem -Path $dir -File
foreach ($file in $children) {
    $args += "`"" + $file.FullName + "`" "
}

$command = "echo `"`" | ./QOIComarisonImprovement.exe " + $args + ">> " + $out

$command | Invoke-Expression