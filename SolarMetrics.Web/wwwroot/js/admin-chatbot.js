(function () {
    "use strict";

    function escapeHtml(text) {
        const div = document.createElement("div");
        div.textContent = text;
        return div.innerHTML;
    }

    function formatMessage(text) {
        return escapeHtml(text).replace(/\n/g, "<br>");
    }

    function initPanel(panel) {
        const askUrl = panel.getAttribute("data-ask-url");
        const messagesEl = panel.querySelector(".chatbot-messages");
        const welcomeEl = panel.querySelector(".chatbot-welcome");
        const suggestionsEl = panel.querySelector(".chatbot-suggestions");
        const inputEl = panel.querySelector(".chatbot-input");
        const sendBtn = panel.querySelector(".chatbot-send");
        const tokenInput = panel.querySelector('input[name="__RequestVerificationToken"]');

        if (!askUrl || !messagesEl || !inputEl || !sendBtn || !tokenInput) {
            return;
        }

        let loading = false;

        function appendBubble(role, text, meta) {
            if (welcomeEl) {
                welcomeEl.classList.add("d-none");
            }
            if (suggestionsEl) {
                suggestionsEl.classList.add("d-none");
            }

            const bubble = document.createElement("div");
            bubble.className = "chatbot-bubble chatbot-bubble--" + role;
            bubble.innerHTML =
                '<div class="chatbot-bubble__text">' + formatMessage(text) + "</div>" +
                (meta ? '<div class="chatbot-bubble__meta small text-muted mt-1">' + escapeHtml(meta) + "</div>" : "");
            messagesEl.appendChild(bubble);
            messagesEl.scrollTop = messagesEl.scrollHeight;
        }

        function setLoading(isLoading) {
            loading = isLoading;
            sendBtn.disabled = isLoading;
            inputEl.disabled = isLoading;
        }

        async function sendQuestion(question) {
            const trimmed = (question || "").trim();
            if (!trimmed || loading) {
                return;
            }

            appendBubble("user", trimmed);
            inputEl.value = "";
            setLoading(true);
            appendBubble("assistant", "Consultando a IA generativa Oracle...", null);

            const loadingBubble = messagesEl.lastElementChild;

            try {
                const response = await fetch(askUrl, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken": tokenInput.value
                    },
                    body: JSON.stringify({ question: trimmed })
                });

                const data = await response.json();

                if (loadingBubble) {
                    loadingBubble.remove();
                }

                if (!response.ok) {
                    appendBubble(
                        "error",
                        data.errorMessage || data.title || "Erro ao consultar o assistente.",
                        null
                    );
                    return;
                }

                if (data.success === false) {
                    appendBubble("error", data.errorMessage || "Não foi possível obter resposta.", null);
                    return;
                }

                const meta = data.source ? "Fonte: " + data.source : "Oracle Select AI";
                appendBubble("assistant", data.answer || "", meta);
            } catch (err) {
                if (loadingBubble) {
                    loadingBubble.remove();
                }
                appendBubble("error", "Erro de rede ao consultar o assistente: " + err.message, null);
            } finally {
                setLoading(false);
                inputEl.focus();
            }
        }

        sendBtn.addEventListener("click", function () {
            sendQuestion(inputEl.value);
        });

        inputEl.addEventListener("keydown", function (e) {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                sendQuestion(inputEl.value);
            }
        });

        panel.querySelectorAll(".chatbot-suggestion").forEach(function (btn) {
            btn.addEventListener("click", function () {
                const q = btn.getAttribute("data-question");
                if (q) {
                    inputEl.value = q;
                    sendQuestion(q);
                }
            });
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll("[data-chatbot-panel]").forEach(initPanel);
    });
})();
