const fs = require("fs");
const { execSync } = require('child_process');
const versions = require("./versions.json");
const { parseStringPromise } = require("xml2js");

const MAX_CONCURRENT_MINOR_VERSIONS = 3;

function versionsWithMajor(major) {
    const ret = [];

    for (const i of versions) {
        if (i.split(".")[0] === major) {
            ret.push(i);
        }
    }

    return ret;
}

function removeVersion(version) {
    console.log(`Removing version ${version}`);

    fs.rmSync(`versioned_docs/version-${version}`, { force: true, recursive: true });
    fs.rmSync(`versioned_sidebars/version-${version}-sidebars.json`);
    versions.splice(versions.indexOf(version), 1);
    fs.writeFileSync("versions.json", JSON.stringify(versions, null, 2));
}

function addVersion(version) {
    execSync(`npm run docusaurus docs:version ${version}`);
}

parseStringPromise(fs.readFileSync("../Amethyst/Amethyst.csproj")).then((currentVersion) => {
    currentVersion = "v" + currentVersion.Project.PropertyGroup[0].Version[0];
    
    if (versions.includes(currentVersion)) {
        console.log("Already built documentation for this version");
        return;
    }

    const major = currentVersion.split(".")[0];
    const minor = currentVersion.split(".")[1];
    const overlap = versionsWithMajor(major);

    var replace = false;
    for (const i of overlap) {
        if (i.split(".")[1] === minor) {
            removeVersion(i);
            replace = true;
            break;
        }
    }

    if (!replace) {
        if (overlap.length >= MAX_CONCURRENT_MINOR_VERSIONS) {
            removeVersion(overlap[overlap.length - 1]);
        }
    }

    addVersion(currentVersion);
});
