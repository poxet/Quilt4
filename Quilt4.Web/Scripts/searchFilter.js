function updateLevelRows() {
    toggleLevelRows("Error");
    toggleLevelRows("Warning");
    toggleLevelRows("Information");
}

function setLevelFilterButtonStyle() {
    var error = getUrl("ErrorFilterButton");
    var warning = getUrl("WarningFilterButton");
    var information = getUrl("InformationFilterButton");

    if (error == "Checked") {
        document.getElementById("ErrorFilterButton").style.border = "4px solid #fff";
    } else {
        document.getElementById("ErrorFilterButton").style.border = "1px solid #000";
    }

    if (warning == "Checked") {
        document.getElementById("WarningFilterButton").style.border = "4px solid #fff";
    } else {
        document.getElementById("WarningFilterButton").style.border = "1px solid #000";
    }

    if (information == "Checked") {
        document.getElementById("InformationFilterButton").style.border = "4px solid #fff";
    } else {
        document.getElementById("InformationFilterButton").style.border = "1px solid #000";
    }

}

function toggleLevelFilterButton(level) {
    if (document.getElementById(level + "FilterButton").style.border == "4px solid rgb(255, 255, 255)") {
        document.getElementById(level + "FilterButton").style.border = "1px solid #000";
        setUrl(level + "FilterButton", "UnChecked");
    } else {
        document.getElementById(level + "FilterButton").style.border = "4px solid #fff";
        setUrl(level + "FilterButton", "Checked");
    }

    toggleLevelRows(level);
}

function toggleLevelRows(level) {
    var rows;
    var elements;
    var checked;
    var i;
    var j;

    if (level == "Error") {
        rows = document.getElementsByClassName("Error");
        for (i = 0; i < rows.length; i++) {
            elements = $(rows[i].parentNode).children("span");
            checked = 0;

            for (j = 0; j < elements.length; j++) {
                if (getUrl(elements[j].className + "FilterButton") == "Checked") {
                    checked++;
                }
            }

            if (checked == elements.length) {
                rows[i].parentNode.parentNode.style.display = "none";

            } else {
                rows[i].parentNode.parentNode.style.display = "table-row";
            }
        }
    }
    if (level == "Warning") {
        rows = document.getElementsByClassName("Warning");
        for (i = 0; i < rows.length; i++) {
            elements = $(rows[i].parentNode).children("span");
            checked = 0;

            for (j = 0; j < elements.length; j++) {
                if (getUrl(elements[j].className + "FilterButton") == "Checked") {
                    checked++;
                }
            }

            if (checked == elements.length) {
                rows[i].parentNode.parentNode.style.display = "none";

            } else {
                rows[i].parentNode.parentNode.style.display = "table-row";
            }
        }
    }
    if (level == "Information") {
        rows = document.getElementsByClassName("Information");
        for (i = 0; i < rows.length; i++) {

            elements = $(rows[i].parentNode).children("span");
            checked = 0;

            for (j = 0; j < elements.length; j++) {
                if (getUrl(elements[j].className + "FilterButton") == "Checked") {
                    checked++;
                }
            }

            if (checked == elements.length) {
                rows[i].parentNode.parentNode.style.display = "none";

            } else {
                rows[i].parentNode.parentNode.style.display = "table-row";
            }
        }
    }
}

function setUrl(name, value) {
    var parameters = window.location.href.split("?").pop();
    var keys = parameters.split("&");

    for (var i = 0; i < keys.length; i++) {
        if (keys[i].indexOf(name.replace("Button", "")) >= 0) {
            var x = keys[i].replace(keys[i].split("=").pop(), value);
            var url = window.location.href.replace(keys[i], x);
            window.history.replaceState("", "", url);
            return;
        }
    }
    window.history.replaceState("", "", window.location.href + "&" + name + "=" + value);
}

function getUrl(name) {
    var parameters = window.location.href.split("?").pop();
    var keys = parameters.split("&");

    for (var i = 0; i < keys.length; i++) {
        if (keys[i].indexOf(name.replace("Button", "")) >= 0) {
            return keys[i].split("=").pop();
        }
    }
}