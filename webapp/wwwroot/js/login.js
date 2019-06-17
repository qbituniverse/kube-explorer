function loadLogin(isError) {
    if (!isError) showGridResults();
    else $(".grid-container").fadeIn(100);

    $('.loginbtn').click(function () {
        clearCatalogMatrix();
        showGridLoading(0, "Checking your access level");
    });
}