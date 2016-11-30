param(
	[string]$SolutionDir,
	[string]$SolutionName,
	[string]$ProjectDir,
	[string]$ProjectName,
	[string]$OutDir,
	[string]$TargetName
)

cp "${ProjectDir}${OutDir}$TargetName.exe" "${SolutionDir}${OutDir}"
cp "${ProjectDir}${OutDir}$TargetName.exe.config" "${SolutionDir}${OutDir}"
cp "${ProjectDir}${OutDir}$TargetName.pdb" "${SolutionDir}${OutDir}"

# No need to reconfigure itself
# & "${SolutionDir}${OutDir}gwconfig.exe" merge $SolutionDir$SolutionName.sln $ProjectName