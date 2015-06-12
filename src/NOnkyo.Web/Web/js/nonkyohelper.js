/// <reference path="_references.js" />
var nonkyohelper = (function () {

    var EVENT_NAMES = {
        commandHub: {
            volumeChanged: "commandHub.volumeChanged",
            inputSelectorChanged: "commandHub.inputSelectorChanged",
            powerStateChanged: "commandHub.powerStateChanged",
            muteStateChanged: "commandHub.muteStateChanged"
        }
    };

    var loGetRequest = function (psUrl) {
        return $.get(psUrl);
    };

    var loGetJson = function (psUrl, poJsonData) {
        $.ajaxSetup({ "cache": false });
        if (poJsonData) {
            return $.getJSON(psUrl, poJsonData);
        } else {
            return $.getJSON(psUrl);
        }
    };

    var loPostJson = function (psUrl, poJsonData) {
        return sendAjaxRequest(psUrl, "POST", poJsonData);
    };

    var loPutJson = function (psUrl, poJsonData) {
        return sendAjaxRequest(psUrl, "PUT", poJsonData);
    };

    var loDeleteJson = function (psUrl, poJsonData) {
        return sendAjaxRequest(psUrl, "DELETE", poJsonData);
    };

    var loDeleteRequest = function (psUrl, pnId) {
        $.ajaxSetup({ "cache": false });
        return $.ajax({
            data: { "_method": "delete" },
            dataType: 'script',
            type: "DELETE",
            url: psUrl + "/" + pnId
        });
    };

    var loOpenWindow = function (psMethode, psUrl, poData, psTarget) {
        var loForm = document.createElement("form");
        loForm.action = psUrl;
        loForm.method = psMethode;
        loForm.target = psTarget || "_self";
        if (poData) {
            for (var lsKey in poData) {
                var loInput = document.createElement("textarea");
                loInput.name = lsKey;
                loInput.value = typeof poData[lsKey] === "object" ? JSON.stringify(poData[lsKey]) : poData[lsKey];
                loForm.appendChild(loInput);
            }
        }
        loForm.style.display = "none";
        document.body.appendChild(loForm);
        loForm.submit();
    };

    var loSetCboValue = function (psCboId, poValue) {
        var loElem = $("#cusel-scroll-" + psCboId);
        var loItem = loElem.find("span[val=" + poValue + "]").first();

        if (!loItem.length)
            return false;

        // invoke value change  
        loItem.click();
    };

    var loStartSignalR = function ($rootscope) {
        var loCommandHub = $.connection.commandHub;

        $.connection.hub.logging = true;

        loCommandHub.client.volumeChanged = function () {
            $rootscope.$broadcast(EVENT_NAMES.commandHub.volumeChanged, arguments);
        };

        loCommandHub.client.inputSelectorChanged = function () {
            $rootscope.$broadcast(EVENT_NAMES.commandHub.inputSelectorChanged, arguments);
        };

        loCommandHub.client.powerStateChanged = function () {
            $rootscope.$broadcast(EVENT_NAMES.commandHub.powerStateChanged, arguments);
        };

        loCommandHub.client.muteStateChanged = function () {
            $rootscope.$broadcast(EVENT_NAMES.commandHub.muteStateChanged, arguments);
        };

        var loPromise = $.connection.hub.start();
        return loPromise;
    };

    var loCreateSlider = function (psIdSelector) {
        var loNewScrollElement = $('#' + psIdSelector).jScrollPane({
            verticalDragMaxHeight: 36,
            verticalDragMinHeight: 36
        });

        loNewScrollElement.addClass('id' + psIdSelector);

        setTimeout(function () {
            loNewScrollElement.find(".jspTrack").append("<div class='jspProgress'></div>");
            $(document).on('jsp-scroll-y', '.scrollbar.style2', function () {
                $scroll_height = $('.scrollbar.style2.id' + psIdSelector + ' .jspDrag').css('top');
                $('.scrollbar.style2.id' + psIdSelector + ' .jspDrag').siblings(".jspProgress").css({ "height": parseInt($scroll_height, 10) + 10 + "px" });
            });
        }, 0);

        return loNewScrollElement;
    };

    //#region Private Funktionen

    function sendAjaxRequest(psUrl, psType, poJsonData) {

        return $.ajax({
            cache: false,
            type: psType,
            url: psUrl,
            contentType: 'application/json',
            dataType: "json",
            data: JSON.stringify(poJsonData)
        });
    }

    //#endregion

    return {
        EVENT_NAMES: EVENT_NAMES,
        getRequest: loGetRequest,
        getJson: loGetJson,
        postJson: loPostJson,
        putJson: loPutJson,
        deleteJson: loDeleteJson,
        deleteRequest: loDeleteRequest,
        openWindow: loOpenWindow,
        setCboValue: loSetCboValue,
        startSignalR: loStartSignalR,
        createSlider: loCreateSlider
    };
}());