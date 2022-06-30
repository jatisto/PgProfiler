function messageBox(array) {
    Swal.fire({
        icon: array.icon,
        title: array.title,
        text: array.message
    })
}

function messageBoxAndRefresh(array) {    
    Swal.fire({
        icon: array.icon,
        title: array.title,
        text: array.message
    }).then(x => {
        window.location.reload();
    })
}


function refreshPage(seconds) {
    setTimeout(function () {
        window.location.reload();
    }, seconds)    
}

function deleteMessageBox(values) {    
    return Swal.fire({
        title: values.title,
        text: values.message,
        icon: values.icon,
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Удалить',
        cancelButtonText: 'Отмена'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire(
                'Удаление',
                'Данные успешно удалены',
                'success'
            )
            return true;            
        }else {
            Swal.fire(
                'Отмена',
                'Данные не удалены',
                'error'
            )
            return false;
        }
    })
}

function jsonStringify(obj) {
    return JSON.stringify(obj);
}

function startTimer(id, seconds, secondsToDisplay) {
    setInterval(function () {
        showElementById(id, secondsToDisplay);
    }, seconds);
}

function showElementById(id, seconds) {
    document.getElementById(id).className = "";
    setTimeout(function () {
        document.getElementById(id).className = "d-none d-print-block";
    }, seconds);
}


async function downloadFileFromStream(array) {
    const arrayBuffer = await array.streamRef.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);

    triggerFileDownload(array.fileName, url);

    URL.revokeObjectURL(url);
}

function triggerFileDownload(fileName, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}