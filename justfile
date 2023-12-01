set windows-shell := ['cmd', '/C']
set shell := ['bash', '-c']

default: run-release

# Run debug build
run-debug:
  dotnet run --project ./src

# Build release build
run-release:
  dotnet run --configuration Release --project ./src
