/// <reference path="_references.js" />
var mainmodule = angular.module('mainmodule', []);

mainmodule.controller('mainController', ['$scope', function ($scope) {
    $scope.data = {};

    //#region PowerState 

    nonkyohelper.getJson(apiRoutes.URLS.ApiPowerGetState)
        .done(function (poResult) {
            $scope.$apply(function () {
                $scope.data.powerstate = poResult === 'ON';
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
        .then(function (poResult) {
            $scope.data.maxvolume = poResult;
            return nonkyohelper.getJson(apiRoutes.URLS.ApiVolumeGetVolume);
        })
        .done(function (poResult) {
            $scope.$apply(function () {
                $scope.data.volume = poResult;
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
        if (!isNaN(lnVolume) && lnVolume != $scope.data.volume) {
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

}]);
