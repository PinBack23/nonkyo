var apiRoutes = (function () {
    var lsRootUrl = 'http://' + location.host + '/api/';

    //http://localhost:9876/api/Volume/GetMaxVolume
    //http://localhost:9876/api/Volume/GetVolume
    //http://localhost:9876/api/Volume/SetVolume?volume=20
    //http://localhost:9876/api/Volume/SetVolumeDown
    //http://localhost:9876/api/Volume/SetVolumeUp

    //http://localhost:9876/api/Power/GetState
    //http://localhost:9876/api/Power/PowerOn
    //http://localhost:9876/api/Power/PowerOff

    //http://localhost:9876/api/InputSelector/GetState
    //http://localhost:9876/api/InputSelector/Set?selector=TUNER
    //selector (description)
    //----------------------------
    //VIDEO1 (VCR/DVR)
    //VIDEO2 (CBL/SAT)
    //VIDEO3 (GAME/TV)
    //VIDEO4 (AUX2)
    //VIDEO5 (AUX2)
    //VIDEO6 (PC)
    //VIDEO7
    //HIDDEN1 (EXTRA 1)
    //HIDDEN2 (EXTRA 2)
    //HIDDEN3 (EXTRA 3)
    //BDDVD (DVD, BD/DVD)
    //TAPE1 (TAPE(1), TV/TAPE)
    //TAPE2
    //PHONO
    //TVCD (TV/CD)
    //FM 
    //AM
    //TUNER
    //MUSICSERVER (DLNA)
    //INTERNETRADIO (iRadio Favorite)
    //USB (USB/USB(Front))
    //USBREAR (USB(Rear))
    //NETWORKNET (NETWORK)
    //USBTOGGLE (USB(toggle))
    //UNIVERSALPORT (Universal PORT)
    //MULTICH (MULTI CH)
    //XM
    //SIRIUS
    //DAB5
    //OFF
    //SOURCE

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