/// <reference path="_references.js" />
var mainmodule = angular.module('mainmodule', ['ui.router']);

mainmodule.config(function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise('/home');

    $stateProvider
        .state('home', {
            url: '/home',
            templateUrl: 'Web/pages/home.html',
            controller: "homeController"
        })
        .state('receiver', {
            url: '/receiver',
            templateUrl: 'Web/pages/receiver.html',
            controller: "receiverController"
        });
});

mainmodule.controller('mainController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    $scope.data = {};
    nonkyohelper.startSignalR($rootScope);
   
    //#region Volume 

    nonkyohelper.getJson(apiRoutes.URLS.ApiVolumeGetVolume)
    .done(function (pnResult) {
        $scope.$apply(function () {
            $scope.data.volume = pnResult;
        });
    })
    .fail(function (error) {
        alert("Cannot read volume:\n" + JSON.stringify(error));
    });

    //loCommandHub.client.volumeChanged = function (pnVolume) {
    //    $scope.$apply(function () {
    //        $scope.data.volume = pnVolume;
    //    });
    //};

    $rootScope.$on(nonkyohelper.EVENT_NAMES.commandHub.volumeChanged, function (event, args) {
        $scope.$apply(function () {
            $scope.data.volume = args[0];
        });
    });

    //#endregion

    //#region InputSelector 

    function readInputSelector() {
        nonkyohelper.getJson(apiRoutes.URLS.ApiInputSelectorGetDescriptionState)
        .done(function (psDescription) {
            $scope.$apply(function () {
                $scope.data.inputselector = psDescription;
            });
        })
        .fail(function (error) {
            alert("Cannot read inputselector:\n" + JSON.stringify(error));
        });
    }

    //loCommandHub.client.inputSelectorChanged = function () {
    //    readInputSelector();
    //};

    $rootScope.$on(nonkyohelper.EVENT_NAMES.commandHub.inputSelectorChanged, function (event, args) {
        readInputSelector();
    });

    readInputSelector();

    //#endregion

}]);

mainmodule.controller('homeController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    $scope.data = {};
    nonkyohelper.startSignalR($rootScope);

    //#region PowerState 

    nonkyohelper.getJson(apiRoutes.URLS.ApiPowerGetState)
        .done(function (psResult) {
            $scope.$apply(function () {
                $scope.data.powerstate = psResult === 'ON';
            });
        })
        .fail(function (error) {
            alert("Cannot read powerstate:\n" + JSON.stringify(error));
        });

    $scope.setPowerstate = function (pbState) {
        if (pbState) {
            nonkyohelper.getRequest(apiRoutes.URLS.ApiPowerPowerOn).done().fail(function (error) {
                alert("Cannot set powerstate:\n" + JSON.stringify(error));
            });
        } else {
            nonkyohelper.getRequest(apiRoutes.URLS.ApiPowerPowerOff).done().fail(function (error) {
                alert("Cannot set powerstate:\n" + JSON.stringify(error));
            });
        }
    };

    $rootScope.$on(nonkyohelper.EVENT_NAMES.commandHub.powerStateChanged, function (event, args) {
        var lbPowerState = args[0];
        if (lbPowerState !== $scope.data.powerstate) {
            $scope.$apply(function () {
                $scope.data.powerstate = lbPowerState;
            });
        }
    });

    //#endregion

    //#region Mute 

    nonkyohelper.getJson(apiRoutes.URLS.ApiAudioMutingGetState)
        .done(function (pbResult) {
            $scope.$apply(function () {
                $scope.data.mutestate = pbResult;
            });
        })
        .fail(function (error) {
            alert("Cannot read mutestate:\n" + JSON.stringify(error));
        });

    $scope.setMutestate = function (pbState) {
        if (pbState) {
            nonkyohelper.getRequest(apiRoutes.URLS.ApiAudioMutingMuteOn).done().fail(function (error) {
                alert("Cannot set mutestate:\n" + JSON.stringify(error));
            });
        } else {
            nonkyohelper.getRequest(apiRoutes.URLS.ApiAudioMutingMuteOff).done().fail(function (error) {
                alert("Cannot set mutestate:\n" + JSON.stringify(error));
            });
        }
    };

    $rootScope.$on(nonkyohelper.EVENT_NAMES.commandHub.muteStateChanged, function (event, args) {
        var lbMuteState = args[0];
        if (lbMuteState !== $scope.data.mutestate) {
            $scope.$apply(function () {
                $scope.data.mutestate = lbMuteState;
            });
        }
    });

    //#endregion

    //#region Volume
    var loTimeoutVolumeHandle;

    var loVolumeScrollElement = nonkyohelper.createSlider('volumeScroll');
    var loVolumeScrollApi = loVolumeScrollElement.data("jsp");

    nonkyohelper.getJson(apiRoutes.URLS.ApiVolumeGetMaxVolume)
        .then(function (pnResult) {
            $scope.data.maxvolume = pnResult;
            return nonkyohelper.getJson(apiRoutes.URLS.ApiVolumeGetVolume);
        })
        .done(function (pnResult) {
            $scope.$apply(function () {
                $scope.data.volume = pnResult;
                loVolumeScrollApi.scrollToPercentY(1 - ($scope.data.volume / $scope.data.maxvolume));
            });
        })
        .fail(function (error) {
            alert("Cannot read volume:\n" + JSON.stringify(error));
        });

    loVolumeScrollElement.bind("jsp-scroll-y", function () {
        clearTimeout(loTimeoutVolumeHandle);
        loTimeoutVolumeHandle = setTimeout(function () {
            var lnNewVolume = Math.floor((1 - loVolumeScrollApi.getPercentScrolledY()) * $scope.data.maxvolume);
            if (lnNewVolume !== $scope.data.volume) {
                nonkyohelper.getRequest(apiRoutes.URLS.ApiVolumeSetVolume + "?volume=" + lnNewVolume).done()
                        .fail(function (error) {
                            alert("Cannot set volume:\n" + JSON.stringify(error));
                        });
            }
        }, 500);
    });

    $rootScope.$on(nonkyohelper.EVENT_NAMES.commandHub.volumeChanged, function (event, args) {
        var lnVolume = args[0];
        if (!isNaN(lnVolume) && lnVolume !== $scope.data.volume) {
            $scope.$apply(function () {
                $scope.data.volume = lnVolume;
                loVolumeScrollApi.scrollToPercentY(1 - ($scope.data.volume / $scope.data.maxvolume));
            });
        }
    });

    $scope.setVolumeUp = function () {
        nonkyohelper.getRequest(apiRoutes.URLS.ApiVolumeSetVolumeUp).done()
                .fail(function (error) {
                    alert("Cannot set volume:\n" + JSON.stringify(error));
                });
    };

    $scope.setVolumeDown = function () {
        nonkyohelper.getRequest(apiRoutes.URLS.ApiVolumeSetVolumeDown).done()
                .fail(function (error) {
                    alert("Cannot set volume:\n" + JSON.stringify(error));
                });
    };

    //#endregion

}]);

mainmodule.controller('receiverController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    $scope.data = {};
    nonkyohelper.startSignalR($rootScope);

    //#region InputSelector

    nonkyohelper.getJson(apiRoutes.URLS.ApiInputSelectorAllSelectors)
        .then(function (poResult) {
            $scope.$apply(function () {
                $scope.data.allinputselectors = poResult;
            });
            return nonkyohelper.getJson(apiRoutes.URLS.ApiInputSelectorGetEnumState);
        })
        .done(function (pnResult) {
            $scope.$apply(function () {
                $scope.data.inputselector = pnResult;
            });
            setTimeout(function () {
                cuSel({ changedEl: "#cboInputselector", visRows: 10 });
                $scope.$apply(function () {
                    $scope.data.inputselectorready = true;
                    nonkyohelper.setCboValue("cboInputselector", $scope.data.inputselector);
                });
            }, 500);

        })
        .fail(function (error) {
            alert("Cannot read inputselector:\n" + JSON.stringify(error));
        });

    $scope.setInputSelector = function (poElement) {
        var lnNewValue = $("#cboInputselector").val();
        if ($scope.data.inputselector !== lnNewValue) {
            $scope.data.inputselector = lnNewValue;
            nonkyohelper.getRequest(apiRoutes.URLS.ApiInputSelectorSetEnum + "?selector=" + $scope.data.inputselector).done()
                .fail(function (error) {
                    alert("Cannot set inputselector:\n" + JSON.stringify(error));
                });
        }
    };

    $scope.setEnumInputSelector = function (psEnumValue) {
        nonkyohelper.getRequest(apiRoutes.URLS.ApiInputSelectorSet + "?selector=" + psEnumValue).done()
            .fail(function (error) {
                alert("Cannot set inputselector:\n" + JSON.stringify(error));
            });
    };

    //loCommandHub.client.inputSelectorChanged = function (pnInputselector) {
    //    if (!isNaN(pnInputselector) && pnInputselector !== $scope.data.inputselector) {
    //        $scope.$apply(function () {
    //            $scope.data.inputselector = pnInputselector;
    //            nonkyohelper.setCboValue("cboInputselector", $scope.data.inputselector);
    //        });
    //    }
    //};

    $rootScope.$on(nonkyohelper.EVENT_NAMES.commandHub.inputSelectorChanged, function (event, args) {
        var lnInputselector = args[0];
        if (!isNaN(lnInputselector) && lnInputselector !== $scope.data.inputselector) {
            $scope.$apply(function () {
                $scope.data.inputselector = lnInputselector;
                nonkyohelper.setCboValue("cboInputselector", $scope.data.inputselector);
            });
        }
    });

    //#endregion

}]);

mainmodule.controller('tempmainController', ['$scope', function ($scope) {
    $scope.data = {};

    $scope.data.signalr = {
        connectionReady: false,
        errorMessage: undefined
    };

    //#region Signal-R Init

    var loCommandHub = $.connection.commandHub;

    // Start the connection.
    $.connection.hub.start().done(function () {
        $scope.$apply(function () {
            $scope.data.signalr.connectionReady = true;
        });
    }).fail(function (error) {
        $scope.data.signalr.connectionReady = false;
        $scope.data.signalr.errorMessage = JSON.stringify(error);
    });

    //#endregion

    //#region PowerState 

    nonkyohelper.getJson(apiRoutes.URLS.ApiPowerGetState)
        .done(function (psResult) {
            $scope.$apply(function () {
                $scope.data.powerstate = psResult === 'ON';
            });
        })
        .fail(function (error) {
            alert("Cannot read powerstate:\n" + JSON.stringify(error));
        });

    $scope.setPowerstate = function (pbState) {
        if (pbState) {
            nonkyohelper.getRequest(apiRoutes.URLS.ApiPowerPowerOn).done().fail(function (error) {
                alert("Cannot set powerstate:\n" + JSON.stringify(error));
            });
        } else {
            nonkyohelper.getRequest(apiRoutes.URLS.ApiPowerPowerOff).done().fail(function (error) {
                alert("Cannot set powerstate:\n" + JSON.stringify(error));
            });
        }
    };

    loCommandHub.client.powerStateChanged = function (pbPowerState) {
        if (pbPowerState !== $scope.data.powerstate) {
            $scope.$apply(function () {
                $scope.data.powerstate = pbPowerState;
            });
        }
    };

    //#endregion

    //#region Volume
    var loTimeoutVolumeHandle;
    var loTimeoutVolumeHandle2;

    var loVolumeScrollElement = $('#volumeScroll').jScrollPane({
        verticalDragMaxHeight: 36,
        verticalDragMinHeight: 36
    });
    var loVolumeScrollApi = loVolumeScrollElement.data("jsp");

    nonkyohelper.getJson(apiRoutes.URLS.ApiVolumeGetMaxVolume)
        .then(function (pnResult) {
            $scope.data.maxvolume = pnResult;
            return nonkyohelper.getJson(apiRoutes.URLS.ApiVolumeGetVolume);
        })
        .done(function (pnResult) {
            $scope.$apply(function () {
                $scope.data.volume = pnResult;
                //$('#txtVolume').knobRot({
                //    'classes': ['volume'],
                //    'dragVertical': false,
                //    'frameCount': 49,
                //    'frameWidth': 149,
                //    'frameHeight': 149,
                //    'detent': true,
                //    'detentThreshold': 5,
                //    'minimumValue': 0,
                //    'maximumValue': $scope.data.maxvolume,
                //    'hideInput': true,
                //    'callback': function () {
                //        clearTimeout(loTimeoutVolumeHandle);
                //        loTimeoutVolumeHandle = setTimeout(setVolume, 500);
                //    }
                //});
                //$('#txtVolume').knobRot("set", $scope.data.volume);
                loVolumeScrollApi.scrollToPercentY(1 - ($scope.data.volume / $scope.data.maxvolume));
            });
        })
        .fail(function (error) {
            alert("Cannot read volume:\n" + JSON.stringify(error));
        });

    //function setVolume() {
    //    var lnVolume = $('#txtVolume').knobRot("getvalue");
    //    if (!isNaN(lnVolume) && lnVolume !== $scope.data.volume) {
    //        $scope.$apply(function () {
    //            $scope.data.volume = lnVolume;
    //            loVolumeScrollApi.scrollToPercentY(1 - ($scope.data.volume / $scope.data.maxvolume));
    //            nonkyohelper.getRequest(apiRoutes.URLS.ApiVolumeSetVolume + "?volume=" + $scope.data.volume).done()
    //                .fail(function (error) {
    //                    alert("Cannot set volume:\n" + JSON.stringify(error));
    //                });
    //        });
    //    }
    //}

    loVolumeScrollElement.bind("jsp-scroll-y", function () {
        clearTimeout(loTimeoutVolumeHandle2);
        loTimeoutVolumeHandle2 = setTimeout(function () {
            var lnNewVolume = Math.floor((1 - loVolumeScrollApi.getPercentScrolledY()) * $scope.data.maxvolume);
            if (lnNewVolume !== $scope.data.volume) {
                nonkyohelper.getRequest(apiRoutes.URLS.ApiVolumeSetVolume + "?volume=" + lnNewVolume).done()
                        .fail(function (error) {
                            alert("Cannot set volume:\n" + JSON.stringify(error));
                        });
            }
        }, 500);
    });

    //loVolumeScrollElement.on("mouseup", function () {
    //    var lnNewVolume = Math.floor((1 - loVolumeScrollApi.getPercentScrolledY()) * $scope.data.maxvolume);
    //    if (lnNewVolume !== $scope.data.volume){
    //        nonkyohelper.getRequest(apiRoutes.URLS.ApiVolumeSetVolume + "?volume=" + lnNewVolume).done()
    //                .fail(function (error) {
    //                    alert("Cannot set volume:\n" + JSON.stringify(error));
    //                });
    //    }
    //});

    loCommandHub.client.volumeChanged = function (pnVolume) {
        if (!isNaN(pnVolume) && pnVolume !== $scope.data.volume) {
            $scope.$apply(function () {
                $scope.data.volume = pnVolume;
                //$('#txtVolume').knobRot("set", $scope.data.volume);
                //$('#txtVolume').trigger('knobrefresh');
                loVolumeScrollApi.scrollToPercentY(1 - ($scope.data.volume / $scope.data.maxvolume));
            });
        }
    };

    //#endregion

    //#region InputSelector

    nonkyohelper.getJson(apiRoutes.URLS.ApiInputSelectorAllSelectors)
        .then(function (poResult) {
            $scope.$apply(function () {
                $scope.data.allinputselectors = poResult;
            });
            return nonkyohelper.getJson(apiRoutes.URLS.ApiInputSelectorGetEnumState);
        })
        .done(function (pnResult) {
            $scope.$apply(function () {
                $scope.data.inputselector = pnResult;
            });
            setTimeout(function () {
                cuSel({ changedEl: "#cboInputselector", visRows: 10 });
                $scope.$apply(function () {
                    $scope.data.inputselectorready = true;
                    nonkyohelper.setCboValue("cboInputselector", $scope.data.inputselector);
                });
            }, 500);

        })
        .fail(function (error) {
            alert("Cannot read inputselector:\n" + JSON.stringify(error));
        });

    $scope.setInputSelector = function (poElement) {
        var lnNewValue = $("#cboInputselector").val();
        if ($scope.data.inputselector !== lnNewValue) {
            $scope.data.inputselector = lnNewValue;
            nonkyohelper.getRequest(apiRoutes.URLS.ApiInputSelectorSetEnum + "?selector=" + $scope.data.inputselector).done()
                .fail(function (error) {
                    alert("Cannot set inputselector:\n" + JSON.stringify(error));
                });
        }
    };

    loCommandHub.client.inputSelectorChanged = function (pnInputselector) {
        if (!isNaN(pnInputselector) && pnInputselector !== $scope.data.inputselector) {
            $scope.$apply(function () {
                $scope.data.inputselector = pnInputselector;
                nonkyohelper.setCboValue("cboInputselector", $scope.data.inputselector);
            });
        }
    };

    //#endregion

    //#region Test

    $scope.showHello = function () {
        //loVolumeScrollApi.scrollToPercentY(0.5);
        //loCommandHub.server.getMessage().done(function (psMessage) {
        //    alert(psMessage);
        //}).fail(function (error) {
        //    alert("Signal-R Error:\n" + JSON.stringify(error));
        //});
    };

    //#endregion
}]);
