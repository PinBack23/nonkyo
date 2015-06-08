/// <reference path="_references.js" />
var mainmodule = angular.module('mainmodule', []);

mainmodule.controller('mainController', ['$scope', function ($scope) {
    $scope.data = {};

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

    //#endregion

    //#region Volume
    var loTimeoutVolumeHandle;

    nonkyohelper.getJson(apiRoutes.URLS.ApiVolumeGetMaxVolume)
        .then(function (pnResult) {
            $scope.data.maxvolume = pnResult;
            return nonkyohelper.getJson(apiRoutes.URLS.ApiVolumeGetVolume);
        })
        .done(function (pnResult) {
            $scope.$apply(function () {
                $scope.data.volume = pnResult;
                $('#txtVolume').knobRot({
                    'classes': ['volume'],
                    'dragVertical': false,
                    'frameCount': 49,
                    'frameWidth': 149,
                    'frameHeight': 149,
                    'detent': true,
                    'detentThreshold': 5,
                    'minimumValue': 0,
                    'maximumValue': $scope.data.maxvolume,
                    'hideInput': true,
                    'callback': function () {
                        clearTimeout(loTimeoutVolumeHandle);
                        loTimeoutVolumeHandle = setTimeout(setVolume, 500);
                    }
                });
                $('#txtVolume').knobRot("set", $scope.data.volume);
            });
        })
        .fail(function (error) {
            alert("Cannot read volume:\n" + JSON.stringify(error));
        });

    function setVolume() {
        var lnVolume = $('#txtVolume').knobRot("getvalue");
        if (!isNaN(lnVolume) && lnVolume !== $scope.data.volume) {
            $scope.$apply(function () {
                $scope.data.volume = lnVolume;
                nonkyohelper.getRequest(apiRoutes.URLS.ApiVolumeSetVolume + "?volume=" + $scope.data.volume).done()
                    .fail(function (error) {
                        alert("Cannot set volume:\n" + JSON.stringify(error));
                    });
            });
        }
    }

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

    //#endregion

    var loCommandHub = $.connection.commandHub;

    // Start the connection.
    $.connection.hub.start().done(function () {
        $scope.$apply(function () {
            $scope.data.messages.push("Hub gestartet!");
        });
    });

    $scope.showHello = function () {
        loCommandHub.server.getMessage().done(function (psMessage) {
            alert(psMessage);
        }).fail(function (error) {
            alert("Signal-R Error:\n" + JSON.stringify(error));
        });
    };

    loCommandHub.client.volumeChange = function (pnVolume) {
        $scope.$apply(function () {
            $scope.data.volume = pnVolume;
            $('#txtVolume').knobRot("set", $scope.data.volume);
            $('#txtVolume').trigger('knobrefresh');
        });
    };
}]);
