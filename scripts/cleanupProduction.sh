#!/bin/sh

# Exit immediately if a command exits with a non-zero status
set -e

df

cd /c/Users/LSProduction/AppData/Local/BloomHarvester

# get the list of book ids from Parse, and split the file into one line per book
curl -X GET \
  -H "X-Parse-Application-Id: ${BloomHarvesterParseAppIdProd}" -G \
  --data-urlencode "keys=objectId" \
  "https://bloom-parse-server-production.azurewebsites.net/parse/classes/books?limit=99999" \
  | sed 's/{"objectId":/\n{"objectId":/g' > allIds.json

# check and double-check for errors in obtaining the book ids
if [ `fgrep '"error":' allIds.json` ]; then cat allIds.json; exit 1; fi
fgrep -q '"objectId":' allIds.json

# delete all the folders whose names are not found in the list of book ids
cd Prod && for f in *; do if [ ! `fgrep "$f" ../allIds.json` ]; then echo deleting $f; rm -rf "$f"; fi; done

df
