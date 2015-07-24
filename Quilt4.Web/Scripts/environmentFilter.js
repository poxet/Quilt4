function toggleEnvironmentFilterButton(environment) {
    if (document.getElementById("Environment-Legend-" + environment).style.border == "4px solid rgb(255, 255, 255)") {
        document.getElementById("Environment-Legend-" + environment).style.border = "1px solid #000";
    } else {
        document.getElementById("Environment-Legend-" + environment).style.border = "4px solid #fff";
    }

    updateRows(environment);
}

function updateRows(environment) {
    var rows = document.getElementsByClassName("Environment-" + environment);

    for (var i = 0; i < rows.length; i++) {
        if (rows[i].parentNode.parentNode.style.display == "none") {
            rows[i].parentNode.parentNode.style.display = "table-row";
        } else {
            rows[i].parentNode.parentNode.style.display = "none";
        }
    }
}