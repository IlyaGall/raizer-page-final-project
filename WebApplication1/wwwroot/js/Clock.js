function updateTime() {
    fetch('?handler=CurrentTime')
        .then(response => response.json())
        .then(data => {
            document.getElementById('currentTime').textContent = `Time: ${data.time}`;
        });
}
setInterval(updateTime, 1000);
updateTime();