/// <reference path="_references.js" />
var mainmodule = angular.module('mainmodule', []);

mainmodule.controller('mainController', ['$scope', function ($scope) {
    $scope.data = {};
    $scope.data.Header = "Hallo Welt";
}]);
