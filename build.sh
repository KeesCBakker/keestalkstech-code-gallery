#!/bin/bash
set -e

#!/bin/bash

# Loop through all subdirectories
for dir in */; do

    pushd "$dir"

    # Check if a .sln file exists in the directory
    SLN="$(find *.sln | head -1)";
    SLN="$dir/$SLN"

    popd

    if [ -f "$SLN" ]; then

        # Get the .sln file name
        echo "Found solution: $SLN"
        
        # Trigger dotnet build
       
        dotnet build "$SLN"


    else
        echo "No solution file found in $dir"
    fi

done
