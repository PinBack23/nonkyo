var apiRoutes = (function () {
    var lsRootUrl = 'http://' + location.host + '/api/';

    //http://192.168.178.204:9876/api/Volume/GetMaxVolume
    //http://192.168.178.204:9876/api/Volume/GetVolume
    //http://192.168.178.204:9876/api/Volume/SetVolume?volume=20
    //http://192.168.178.204:9876/api/Volume/SetVolumeDown
    //http://192.168.178.204:9876/api/Volume/SetVolumeUp

    //http://192.168.178.204:9876/api/Power/GetState
    //http://192.168.178.204:9876/api/Power/PowerOn
    //http://192.168.178.204:9876/api/Power/PowerOff

    var URLS = {
        ApiVolumeGetMaxVolume: undefined,
        ApiVolumeGetVolume: undefined,
        ApiVolumeSetVolume: undefined,
        ApiVolumeSetVolumeDown: undefined,
        ApiVolumeSetVolumeUp: undefined,

        ApiPowerGetState: undefined,
        ApiPowerPowerOn: undefined,
        ApiPowerPowerOff: undefined
    };

    URLS.ApiVolumeGetMaxVolume = lsRootUrl + "Volume/GetMaxVolume";
    URLS.ApiVolumeGetVolume = lsRootUrl + "Volume/GetVolume";
    URLS.ApiVolumeSetVolume = lsRootUrl + "Volume/SetVolume";
    URLS.ApiVolumeSetVolumeDown = lsRootUrl + "Volume/SetVolumeDown";
    URLS.ApiVolumeSetVolumeUp = lsRootUrl + "Volume/SetVolumeUp";

    URLS.ApiPowerGetState = lsRootUrl + "Power/GetState";
    URLS.ApiPowerPowerOn = lsRootUrl + "Power/PowerOn";
    URLS.ApiPowerPowerOff = lsRootUrl + "Power/PowerOff";

    return {
        URLS: URLS
    };
}());