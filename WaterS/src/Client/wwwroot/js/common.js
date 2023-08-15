ShowToastr = (type, message) => {
    if (type === 'success') {
        toastr.success(message, 'HotelApp', { timeout: 2000 });
    }
    if (type === 'error') {
        toastr.error(message, 'HotelApp', { timeout: 2000 });
    }
}

SwalConfirm = () => {
    return new Promise(resolve => {
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            resolve(result.isConfirmed);
        })
    });
}
initializeCarousel = () => {
    var online = window.navigator.onLine;
    alert(online);
}