function updateLevelRows() {
    toggleLevelRows("Error");
    toggleLevelRows("Warning");
    toggleLevelRows("Information");
}

function setLevelFilterButtonStyle() {
    var error = getCookie("ErrorFilterButton");
    var warning = getCookie("WarningFilterButton");
    var information = getCookie("InformationFilterButton");

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
        setCookie(level + "FilterButton", "UnChecked", 3);
    } else {
        document.getElementById(level + "FilterButton").style.border = "4px solid #fff";
        setCookie(level + "FilterButton", "Checked", 3);
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
                if (getCookie(elements[j].className + "FilterButton") == "Checked") {
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
                if (getCookie(elements[j].className + "FilterButton") == "Checked") {
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
                if (getCookie(elements[j].className + "FilterButton") == "Checked") {
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

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}