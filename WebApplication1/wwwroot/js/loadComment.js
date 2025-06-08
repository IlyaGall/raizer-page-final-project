class CommentService {
    static async addComment(productId, text) {
        const token = localStorage.getItem('jwtToken');
        if (!token) throw new Error('Требуется авторизация');

        const response = await fetch('https://localhost:7052/api/Comment/Add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
                ProductId: productId,
                Text: text
            })
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error);
        }

        return await response.json();
    }

    static async getComments(productId) {
        const response = await fetch(`https://localhost:7052/api/Comment/GetCommentsProduct?productId=${productId}`);

        if (!response.ok) {
            throw new Error('Ошибка загрузки комментариев');
        }

        return await response.json();
    }
}


// Добавление комментария
async function addCommentHandler() {
    try {
        const productId = document.getElementById('productId').value;
        const commentText = document.getElementById('commentText').value;

        const result = await CommentService.addComment(productId, commentText);
        alert('Комментарий добавлен! ID: ' + result);

        // Обновляем список комментариев
        await loadComments();
    } catch (error) {
        console.error('Ошибка:', error);
        alert(error.message);
    }
}

// Загрузка комментариев
async function loadComments() {
    try {

        let response = await fetch(`https://localhost:7276/api/Comment/GetCommentsProduct?IdProduct=1`);

        if (response.ok) { // если HTTP-статус в диапазоне 200-299
            // получаем тело ответа (см. про этот метод ниже)
            let json = await response.json();
            console.log(json);
        } else {
            alert("Ошибка HTTP: " + response.status);
        }
        return;
        const productId = document.getElementById('productId').value;
        const comments = await CommentService.getComments(productId);


        const container = document.getElementById('commentsContainer');
        container.innerHTML = comments.map(c => `
            <div class="comment">
                <p>${c.text}</p>
                <small>Автор: ${c.userName}</small>
            </div>
        `).join('');
    } catch (error) {
        console.error('Ошибка загрузки:', error);
    }
}

// Инициализация
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('addCommentBtn').addEventListener('click', addCommentHandler);
    loadComments();
});