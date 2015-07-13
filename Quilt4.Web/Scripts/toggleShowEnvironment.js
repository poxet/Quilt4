function toggleShowEnvironment(environment) {

    var rows = document.getElementsByClassName(environment + "Color");

    for (var i = 0; i < rows.length; i++) {
        if (rows[i].parentNode.parentNode.style.display == "table-row") {
            rows[i].parentNode.parentNode.style.display = "none";

        } else {
            rows[i].parentNode.parentNode.style.display = "table-row";
        }
    }
}

function toggleShowIssueLevel(level) {
    var rows = document.getElementsByClassName(level);

    for (var i = 0; i < rows.length; i++) {
        if (rows[i].parentNode.parentNode.style.display == "table-row") {
            rows[i].parentNode.parentNode.style.display = "none";

        } else {
            rows[i].parentNode.parentNode.style.display = "table-row";
        }
    }
}

function changeButtonStyle(id) {

    if (document.getElementById(id + "LegendButton").style.border == "4px solid rgb(255, 255, 255)") {
        document.getElementById(id + "LegendButton").style.border = "1px solid #000";
    } else {
        document.getElementById(id + "LegendButton").style.border = "4px solid #fff";
    }
}