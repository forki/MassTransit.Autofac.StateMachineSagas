// include Fake libs
#r "src/packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Paket
open Fake.FileUtils
open Fake.Testing.XUnit2

// Directories
let rootDir = currentDirectory
let buildDir  = currentDirectory + "/bin/"
let testsDir  = currentDirectory + "/tests/"
let nugetDir = currentDirectory + "/nuget/"
let testOutputDir = currentDirectory + "/"

let nugetApiKey = environVar "BAMBOO_nugetApiKey"
let nugetPushUrl = environVarOrDefault "BAMBOO_nugetPublishUrl"
let nugetVersion = getBuildParamOrDefault "nugetVersion" null

// Filesets
let appReferences  =
    !! "src/**/*.csproj"
      -- "src/**/*.Tests.csproj"

let testReferences  =
    !! "src/**/*.Tests.csproj"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; nugetDir]
    MSBuildRelease buildDir "Clean" appReferences
        |> Log "Clean-Output: "
)

Target "BuildTests" (fun _ ->
    MSBuildRelease testsDir "Build" testReferences
        |> Log "BuildTests-Output: "
)

Target "BuildApp" (fun _ ->
    MSBuildRelease buildDir "Build" appReferences
        |> Log "BuildApp-Output: "
)

Target "Push" (fun _ ->
    Push (fun p ->
        {p with
            ApiKey = nugetApiKey
            PublishUrl = nugetPushUrl(null)
            WorkingDir = nugetDir
        })
)

Target "Pack" (fun _ ->
    Pack (fun p ->
        let p' = {p with OutputPath = nugetDir; WorkingDir = rootDir + "/src" }
        if nugetVersion = null then p' else {p' with Version = nugetVersion }
        )
)

Target "Test" (fun _ ->
    !! (testsDir @@ "/*.Tests.dll")
    |> xUnit2 (fun p ->
                { p with
                    NUnitXmlOutputPath = Some (testOutputDir + "testresults.xml");
                    ToolPath = @"src/packages/xunit.runner.console/tools/xunit.console.exe"
                })
)

// Build order
"Clean"
  ==> "BuildApp"
  ==> "BuildTests"
  ==> "Test"
  ==> "Pack"
  ==> "Push"

// start build
RunTargetOrDefault "Test"
