/// <reference path="_references.js" />
var nonkyohelper = (function () {

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
        getRequest: loGetRequest,
        getJson: loGetJson,
        postJson: loPostJson,
        putJson: loPutJson,
        deleteJson: loDeleteJson,
        deleteRequest: loDeleteRequest,
        openWindow: loOpenWindow,
        setCboValue: loSetCboValue
    };
}());