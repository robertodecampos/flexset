function beginLoadingButton(button, icon) {
    $(button).attr("disabled", "true");
    if ($(button).find(".glyphicon").length != 0)
        $(button).find(".glyphicon").removeClass(icon).addClass("glyphicon-refresh glyphicon-refresh-animate");
}

function endLoadingButton(button, icon) {
    if ($(button).find(".glyphicon").length != 0)
        $(button).find(".glyphicon").removeClass("glyphicon-refresh glyphicon-refresh-animate").addClass(icon);
    $(button).removeAttr("disabled");
}

function arrayToUrlParameters(array) {
    if (!Array.isArray(array))
        return '';

    let parameters = '';

    for (let i = 0; i < array.length; i++) {
        if (!Array.isArray(array[i]))
            return;

        if (array[i].length != 2)
            return;

        if (i > 0)
            parameters += '&';
        if (Array.isArray(array[i][1]))
            parameters += arrayToUrlParameters(array[i][1]);
        else
            parameters += array[i][0] + '=' + array[i][1];
    };

    return parameters;
}