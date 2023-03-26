import { FileSystem, Configuration, Environment, Network, Logger } from 'installer'
import { operationSequence } from './helpers.js'

async function run() {
    const { result: configFile } = await FileSystem.readTextFile(`project.json`);
    if (!configFile) return;

    const { result: xmlData } = await Network.getAsString("https://raw.githubusercontent.com/trueromanus/ArdorQuery/main/test.xml");
    Logger.log(xmlData);

    const { result: downloaded } = await Network.getDownloadFile("https://github.githubassets.com/assets/app_assets_modules_github_command-palette_items_help-item_ts-app_assets_modules_github_comman-48ad9d-caa3c9446740.js", "C:/work", "test.js");
    if (!downloaded) return;

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