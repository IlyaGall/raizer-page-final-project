//авторизация 
document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const username = document.getElementById('Username').value;
    const password = document.getElementById('Password').value;

    try {
        await AuthService.login(username, password);
        window.location.href = '/';
    } catch (error) {
        alert(error.message);
    }
});


