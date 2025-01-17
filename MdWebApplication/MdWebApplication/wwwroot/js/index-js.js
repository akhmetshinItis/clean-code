// Инициализация CodeMirror для Markdown
const markdownEditor = CodeMirror.fromTextArea(document.getElementById("markdown-editor"), {
    mode: "markdown",
    lineNumbers: true,
    theme: "material",
});

// Инициализация CodeMirror для HTML
const htmlEditor = CodeMirror.fromTextArea(document.getElementById("html-editor"), {
    mode: "xml",
    lineNumbers: true,
    readOnly: true,
    theme: "material",
});

// Функция для автоматической конвертации Markdown в HTML
markdownEditor.on("change", async () => {
    const markdownInput = markdownEditor.getValue(); // Получаем содержимое Markdown редактора
    try {
        const response = await fetch('/api/Md/convert', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ markdown: markdownInput }),
        });

        if (response.ok) {
            const data = await response.json();
            htmlEditor.setValue(data.html); // Устанавливаем результат в HTML редактор
        } else {
            const error = await response.json();
            console.error('Error:', error.error);
        }
    } catch (err) {
        console.error('Failed to convert Markdown:', err);
    }
});

// Функция для извлечения значения cookie по имени
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

// Функция для скачивания документа
document.getElementById('download-btn').addEventListener('click', async () => {
    const userId = getCookie('userId');  // Извлекаем userId из cookies
    const fileId = 'some-file-id';       // Здесь нужно указать реальный fileId, например, передать его через параметры или форму

    if (!userId) {
        alert('User ID is missing in cookies!');
        return;
    }

    try {            const response = await fetch(`/api/Md/download?userId=${userId}&fileId=${fileId}`, {
        method: 'GET',
    });

        if (response.ok) {
            const markdownContent = await response.text(); // Получаем Markdown содержимое
            markdownEditor.setValue(markdownContent); // Вставляем содержимое в редактор Markdown
        } else {
            const error = await response.json();
            console.error('Error:', error.error);
        }
    } catch (err) {
        console.error('Failed to download document:', err);
    }
});

