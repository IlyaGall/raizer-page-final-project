//аутификация через сервис

class AuthService {
    // Логин через API
    static async login(username, password) {
        console.log(username + " " + password);
        try {

            const response = await fetch('https://localhost:7018/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) 
            {
                throw new Error('Ошибка авторизации');
            }

            const data = await response.json();

            // Сохраняем токен в localStorage
            localStorage.setItem('jwtToken', data.token);

            // Возвращаем данные пользователя
            return data.user;


        }
        catch (error)
        {
            console.error('Login error:', error);
            throw error;
        }
    }

    // Получение информации о пользователе
    async getUserInfo() {
        const token = localStorage.getItem('jwtToken');

        if (!token) return null;

        try {
            const response = await fetch('/api/auth/userinfo', {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                this.logout();
                return null;
            }

            return await response.json();
        } catch (error) {
            console.error('Get user info error:', error);
            return null;
        }
    }

    // Выход
    logout() {
        localStorage.removeItem('jwtToken');
    }

    // Проверка аутентификации
    async isAuthenticated() {
        const userInfo = await this.getUserInfo();
        return userInfo !== null;
    }
}


// Делаем сервис глобально доступным (без модулей) в противном случае не работает
window.AuthService = AuthService;