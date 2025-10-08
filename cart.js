// ДОБАВЬ ЭТОТ КОД В САМОЕ НАЧАЛО cart.js
console.log('🔧 cart.js загружен!');

// Переопределим функцию для дебага
async function calculateDiscountOnBackend(totalPrice, totalTons) {
    console.log('🔄 Вызов calculateDiscountOnBackend:', { totalPrice, totalTons });

    try {
        const response = await fetch('/api/products/calculate-discount', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                totalPrice: totalPrice,
                totalTons: totalTons
            })
        });

        console.log('📡 Ответ от сервера:', response.status, response.statusText);

        if (response.ok) {
            const result = await response.json();
            console.log('✅ Результат скидки:', result);
            return result;
        } else {
            console.error('❌ Ошибка сервера:', response.status);
            throw new Error('Ошибка расчета скидки');
        }
    } catch (error) {
        console.error('💥 Ошибка в calculateDiscountOnBackend:', error);
        return calculateDiscountClient(totalPrice, totalTons);
    }
}

// Функции для страницы cart.html
async function loadCart() {
    try {
        const response = await fetch('/api/cart');
        const cartItems = await response.json();
        await displayCart(cartItems);
    } catch (error) {
        console.error('Ошибка при загрузке корзины:', error);
        document.getElementById('cart-container').innerHTML =
            '<div class="error">❌ Ошибка загрузки корзины</div>';
    }
}

// Функция для расчета скидки через бэкенд
async function calculateDiscountOnBackend(totalPrice, totalTons) {
    try {
        const response = await fetch('/api/products/calculate-discount', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                totalPrice: totalPrice,
                totalTons: totalTons
            })
        });

        if (response.ok) {
            return await response.json();
        } else {
            throw new Error('Ошибка расчета скидки');
        }
    } catch (error) {
        console.error('Ошибка расчета скидки:', error);
        // Fallback на клиентский расчет
        return calculateDiscountClient(totalPrice, totalTons);
    }
}

// Клиентский расчет скидки (на случай если бэкенд недоступен)
function calculateDiscountClient(totalPrice, totalTons) {
    const discountRules = [
        { minTons: 10, discountPercent: 3 },
        { minTons: 50, discountPercent: 7 },
        { minTons: 100, discountPercent: 10 },
        { minTons: 200, discountPercent: 15 }
    ];

    // Находим подходящее правило скидки
    const applicableRule = discountRules
        .filter(rule => totalTons >= rule.minTons)
        .sort((a, b) => b.minTons - a.minTons)[0];

    const discountPercent = applicableRule ? applicableRule.discountPercent : 0;
    const discountAmount = totalPrice * (discountPercent / 100);

    // Находим следующую скидку
    const nextRule = discountRules
        .filter(rule => rule.minTons > totalTons)
        .sort((a, b) => a.minTons - b.minTons)[0];

    const nextDiscount = nextRule ? {
        tonsNeeded: nextRule.minTons - totalTons,
        discountPercent: nextRule.discountPercent
    } : null;

    return {
        discountPercent,
        discountAmount,
        finalPrice: totalPrice - discountAmount,
        nextDiscount
    };
}

// Функция для расчета скидки на отдельный товар
async function calculateItemDiscount(itemPrice, itemTons) {
    return await calculateDiscountOnBackend(itemPrice, itemTons);
}

async function displayCart(cartItems) {
    const cartContainer = document.getElementById('cart-container');
    if (!cartContainer) return;

    addCartStyles();

    if (cartItems.length === 0) {
        cartContainer.innerHTML = '<div class="empty-cart">🛒 Корзина пуста</div>';
        return;
    }

    let totalPrice = 0;
    let totalTons = 0;

    // Считаем общие суммы
    cartItems.forEach(item => {
        totalPrice += item.price * item.quantityTons;
        totalTons += item.quantityTons;
    });

    console.log('🔍 ДЕБАГ: перед расчетом скидки', { totalPrice, totalTons });

    // Получаем скидку
    const discountResult = await calculateDiscountOnBackend(totalPrice, totalTons);

    console.log('🔍 ДЕБАГ: результат скидки', discountResult);

    // ПРОСТОЙ HTML для тестирования
    const cartHTML = `
        <div class="cart-items">
            ${cartItems.map(item => {
        const itemPrice = item.price * item.quantityTons;
        return `
                    <div class="cart-item">
                        <div class="cart-item-name">${item.productName}</div>
                        <div>Количество: ${item.quantityTons} т</div>
                        <div>Цена: ${itemPrice} руб</div>
                    </div>
                `;
    }).join('')}
        </div>
        <div class="cart-total">
            <h3>ИТОГО:</h3>
            <div>Сумма: ${totalPrice} руб</div>
            <div style="color: green;">Скидка: ${discountResult.discountPercent}%</div>
            <div style="color: green;">Сумма скидки: -${discountResult.discountAmount} руб</div>
            <div style="font-weight: bold; font-size: 20px;">К оплате: ${discountResult.finalPrice} руб</div>
            
            ${discountResult.nextDiscount ? `
                <div style="background: yellow; padding: 10px; margin: 10px 0;">
                    До скидки ${discountResult.nextDiscount.discountPercent}% нужно еще ${discountResult.nextDiscount.tonsNeeded} т
                </div>
            ` : ''}
        </div>
    `;

    cartContainer.innerHTML = cartHTML;
}

// Функция для добавления стилей
function addCartStyles() {
    if (document.getElementById('cart-styles')) return;

    const styles = `
        <style id="cart-styles">
            .cart-item {
                background: white;
                border: 2px solid #B87333;
                border-radius: 12px;
                padding: 16px;
                margin-bottom: 16px;
                box-shadow: 0 2px 8px rgba(184, 115, 51, 0.1);
            }

            .cart-item-header {
                display: flex;
                justify-content: space-between;
                align-items: flex-start;
                margin-bottom: 12px;
            }

            .cart-item-name {
                font-size: 16px;
                font-weight: 700;
                color: #B87333;
                flex: 1;
            }

            .discount-badge {
                background: linear-gradient(135deg, #27ae60, #2ecc71);
                color: white;
                padding: 4px 8px;
                border-radius: 12px;
                font-size: 12px;
                font-weight: 700;
            }

            .cart-item-quantity {
                font-size: 14px;
                color: #666;
                margin-bottom: 12px;
                background: #f8f9fa;
                padding: 8px 12px;
                border-radius: 6px;
            }

            .price-with-discount {
                display: flex;
                flex-direction: column;
                align-items: center;
                gap: 4px;
            }

            .original-price {
                text-decoration: line-through;
                color: #666;
                font-size: 14px;
            }

            .discounted-price {
                color: #27ae60;
                font-weight: 700;
                font-size: 16px;
            }

            .regular-price {
                color: #000;
                font-weight: 600;
                font-size: 16px;
            }

            .item-discount-info {
                font-size: 12px;
                color: #27ae60;
                margin-bottom: 12px;
                text-align: center;
                background: rgba(39, 174, 96, 0.1);
                padding: 6px 12px;
                border-radius: 6px;
            }

            .cart-actions {
                display: flex;
                gap: 8px;
            }

            .button {
                background: #B87333;
                color: white;
                border: none;
                padding: 12px 16px;
                border-radius: 8px;
                font-size: 14px;
                font-weight: 600;
                cursor: pointer;
                transition: all 0.3s ease;
                flex: 1;
            }

            .button:hover {
                background: #9A5C27;
            }

            .button-secondary {
                background: white;
                color: #B87333;
                border: 2px solid #B87333;
            }

            .button-secondary:hover {
                background: #f8f9fa;
            }

            .cart-total {
                background: #f8f9fa;
                border: 2px solid #B87333;
                border-radius: 12px;
                padding: 20px;
                margin-top: 20px;
            }

            .total-title {
                font-size: 18px;
                font-weight: 600;
                color: #000;
                margin-bottom: 16px;
                text-align: center;
            }

            .price-row, .discount-row, .final-row {
                display: flex;
                justify-content: space-between;
                margin-bottom: 8px;
                padding: 8px 0;
                border-bottom: 1px solid #e9ecef;
            }

            .discount-row {
                color: #27ae60;
                font-weight: 600;
            }

            .final-row {
                border-bottom: none;
                font-size: 18px;
                font-weight: 700;
                color: #000;
            }

            .final-amount {
                font-size: 24px;
                font-weight: 800;
                color: #B87333;
            }

            .next-discount-info {
                background: rgba(184, 115, 51, 0.1);
                padding: 12px;
                border-radius: 8px;
                margin: 16px 0;
                text-align: center;
                font-size: 14px;
                color: #B87333;
                font-weight: 600;
            }

            .checkout-button {
                background: linear-gradient(135deg, #27ae60, #2ecc71);
                color: white;
                border: none;
                padding: 16px;
                border-radius: 8px;
                font-size: 16px;
                font-weight: 700;
                cursor: pointer;
                width: 100%;
                margin-bottom: 10px;
                transition: all 0.3s ease;
            }

            .checkout-button:hover {
                background: linear-gradient(135deg, #219653, #27ae60);
            }

            .clear-button {
                background: white;
                color: #B87333;
                border: 2px solid #B87333;
                padding: 16px;
                border-radius: 8px;
                font-size: 16px;
                font-weight: 700;
                cursor: pointer;
                width: 100%;
                transition: all 0.3s ease;
            }

            .clear-button:hover {
                background: #f8f9fa;
            }

            .empty-cart {
                text-align: center;
                padding: 40px;
                color: #B87333;
                font-size: 16px;
                font-weight: 600;
            }

            .error {
                text-align: center;
                padding: 20px;
                color: #e74c3c;
                background: rgba(231, 76, 60, 0.1);
                border-radius: 8px;
                margin: 16px 0;
                font-weight: 600;
            }
        </style>
    `;

    document.head.insertAdjacentHTML('beforeend', styles);
}

async function removeFromCart(productId) {
    try {
        const response = await fetch(`/api/cart/${productId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            await loadCart();
        }
    } catch (error) {
        console.error('Ошибка при удалении из корзины:', error);
        alert('❌ Ошибка при удалении товара');
    }
}

async function clearCart() {
    if (!confirm('❓ Вы уверены, что хотите очистить корзину?')) return;

    try {
        const response = await fetch('/api/cart/clear', {
            method: 'POST'
        });

        if (response.ok) {
            await loadCart();
        }
    } catch (error) {
        console.error('Ошибка при очистке корзины:', error);
        alert('❌ Ошибка при очистке корзины');
    }
}

// Функция оформления заказа
async function checkout() {
    try {
        const response = await fetch('/api/cart');
        const cartItems = await response.json();

        if (cartItems.length === 0) {
            alert('❌ Корзина пуста');
            return;
        }

        // Рассчитываем итоговую сумму со скидкой
        let totalPrice = 0;
        let totalTons = 0;
        cartItems.forEach(item => {
            totalPrice += item.price * item.quantityTons;
            totalTons += item.quantityTons;
        });

        const discountResult = await calculateDiscountOnBackend(totalPrice, totalTons);

        const orderData = {
            items: cartItems,
            totalTons: totalTons,
            totalPrice: totalPrice,
            discountPercent: discountResult.discountPercent,
            discountAmount: discountResult.discountAmount,
            finalPrice: discountResult.finalPrice,
            orderDate: new Date().toISOString()
        };

        // Отправляем заказ на сервер
        const orderResponse = await fetch('/api/orders', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(orderData)
        });

        if (orderResponse.ok) {
            const result = await orderResponse.json();
            alert(`✅ Заказ успешно оформлен!\nНомер заказа: ${result.orderId}\nИтоговая сумма: ${result.finalPrice.toFixed(2)} руб`);
            await clearCart();
        } else {
            const error = await orderResponse.json();
            throw new Error(error.error || 'Ошибка при создании заказа');
        }

    } catch (error) {
        console.error('Ошибка при оформлении заказа:', error);
        alert('❌ Ошибка при оформлении заказа: ' + error.message);
    }
}

function openOrderForm() {
    if (cartItems.length === 0) {
        alert('Корзина пуста!');
        return;
    }
    document.getElementById('orderModal').style.display = 'block';
}

function closeOrderForm() {
    document.getElementById('orderModal').style.display = 'none';
}

async function submitOrderForm(event) {
    event.preventDefault();

    const formData = {
        fullName: document.getElementById('fullName').value,
        inn: document.getElementById('inn').value,
        phone: document.getElementById('phone').value,
        email: document.getElementById('email').value,
        comment: document.getElementById('comment').value
    };

    if (!formData.fullName || !formData.inn || !formData.phone || !formData.email) {
        alert('Заполните все поля!');
        return;
    }

    try {
        const response = await fetch('/api/orders', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                customer: formData,
                items: cartItems,
                orderDate: new Date().toISOString()
            })
        });

        if (response.ok) {
            // ✅ ОТПРАВКА ДАННЫХ В TELEGRAM
            const tg = window.Telegram.WebApp;
            tg.showPopup({
                title: '✅ Заказ оформлен!',
                message: 'Спасибо за заказ! Менеджер свяжется с вами в ближайшее время.',
                buttons: [{ type: 'ok' }]
            }, () => {
                // Закрываем приложение после подтверждения
                tg.close();
            });

            await clearCart();
            closeOrderForm();
        } else {
            alert('Ошибка оформления');
        }
    } catch (error) {
        alert('Ошибка сети');
    }
}

// Инициализация для cart.html
document.addEventListener('DOMContentLoaded', function () {
    if (document.getElementById('cart-container')) {
        loadCart();
    }
});

