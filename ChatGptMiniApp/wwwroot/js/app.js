window.ShowModal = (modalId) => {
    $(`#${modalId}`).modal('show');
};

window.HideModal = (modalId) => {
    $(`#${modalId}`).modal('hide');
};
