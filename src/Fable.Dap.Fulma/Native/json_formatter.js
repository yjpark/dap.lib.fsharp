var JSONFormatter = require ('json-formatter-js').default

let render_into_target = function (target, html) {
    var target_node = document.getElementById(target);
    if (target_node != null) {
        var last;
        while (last = target_node.lastChild) target_node.removeChild(last);
        target_node.appendChild(html);
        return true;
    }
    return false;
}

let render_into_target_with_retry = function (target, html, delay, count, max) {
    if (!render_into_target(target, html)) {
        if (count + 1 < max) {
            setTimeout(function() {
                render_into_target_with_retry(target, html, delay, count + 1, max);
            }, delay);
        } else {
            console.error ("renderJson failed", target, html, delay, max)
        }
    }
}

let renderJsonWithConfig = function (target, json, openDepth, config) {
    const formatter = new JSONFormatter(json, openDepth, config);
    var html = formatter.render();
    render_into_target_with_retry(target, html, 50, 0, 10);
    return html;
}

let defaultConfig =
    {
        hoverPreviewEnabled: true,
        hoverPreviewArrayCount: 100,
        hoverPreviewFieldCount: 5,
        theme: 'dark',
        animateOpen: true,
        animateClose: true,
        useToJSON: true
    }

let renderJsonWithDefault = function (target, json, openDepth) {
    return renderJsonWithConfig(target, json, openDepth, defaultConfig);
}

let renderJsonTemp = function (json) {
    return renderJsonWithConfig("Json_Formatter_Target", json, 20, defaultConfig);
}

module.exports = {
    renderJsonWithConfig: renderJsonWithConfig,
    renderJsonWithDefault: renderJsonWithDefault,
    renderJsonTemp: renderJsonTemp,
    openUrl: function (url, name) {
        window.open(url, name);
    }
};
