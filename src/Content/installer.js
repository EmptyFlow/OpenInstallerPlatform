import { FileSystem, Configuration, Environment } from 'installer'

async function operationSequence(operations) {
    for (const operation of operations) {
        const { result } = await operation();
        if (!result) break;
    }
}

async function run() {
    const { result: configFile } = await FileSystem.readTextFile(`project.json`);
    if (!configFile) return;

    const installerData = JSON.parse(configFile);
    const { result: configured } = await Configuration.configure(installerData.name, installerData.version, installerData.unique);
    if (!configured) return;

    if (Configuration.isEmptyApplicationFolders()) {
        Configuration.addApplicationFolder(`appfolder`, Environment.isWindows ? Environment.programFiles(installerData.name) : Environment.profileFolder(installerData.name));
    }

    await operationSequence(
        [
            () => FileSystem.copyFolder(`Application`, Configuration.applicationFolder(`appfolder`))
        ]
    );
}

export default run;