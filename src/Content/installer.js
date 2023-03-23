import { FileSystem, Configuration, Environment, Network, Logger } from 'installer'

async function operationSequence(operations) {
    for (const operation of operations) {
        const { result } = await operation();
        if (!result) break;
    }
}

async function run() {
    const { result: configFile } = await FileSystem.readTextFile(`project.json`);
    if (!configFile) return;

    const { result: xmlData } = await Network.getAsString("https://raw.githubusercontent.com/trueromanus/ArdorQuery/main/test.xml");
    Logger.log(xmlData);

    const installerData = JSON.parse(configFile);
    const { result: configured } = await Configuration.configure(installerData.name, installerData.version, installerData.unique);
    if (!configured) return;

    if (Configuration.isEmptyApplicationFolders()) {
        Configuration.addApplicationFolder(`appfolder`, Environment.isWindows ? Environment.programFiles(installerData.name) : Environment.profileFolder(installerData.name));
    }

    await operationSequence(
        [
            () => FileSystem.copyFolder(`Application`, Configuration.applicationFolder(`appfolder`)),
            () => FileSystem.copyFile(`Application/README.md`, Configuration.applicationFolder(`appfolder`), "NEWREADME.md")
        ]
    );
}

export default run;