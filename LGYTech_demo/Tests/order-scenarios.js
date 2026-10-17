----------A-----------
async function testSuccessfulOrder() {
    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    )?.value;

    if (!token) {
        throw new Error("Anti-forgery token not found.");
    }

    const data = new URLSearchParams();

    data.append("__RequestVerificationToken", token);
    data.append("CustomerName", "Test Scenario A");

    data.append("Items[0].ProductId", "1");
    data.append("Items[0].Quantity", "2");

    data.append("Items[1].ProductId", "2");
    data.append("Items[1].Quantity", "5");

    const response = await fetch("/Orders/Create", {
        method: "POST",
        credentials: "same-origin",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
        },
        body: data.toString()
    });

    if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
    }

    return await response.json();
}

testSuccessfulOrder();

----------B-----------

async function testRollbackOrder() {
    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    )?.value;

    if (!token) {
        throw new Error("Anti-forgery token not found.");
    }

    const data = new URLSearchParams();

    data.append("__RequestVerificationToken", token);
    data.append("CustomerName", "Test Scenario B");

    data.append("Items[0].ProductId", "1");
    data.append("Items[0].Quantity", "2");

    data.append("Items[1].ProductId", "3");
    data.append("Items[1].Quantity", "999999");

    const response = await fetch("/Orders/Create", {
        method: "POST",
        credentials: "same-origin",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
        },
        body: data.toString()
    });

    if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
    }

    return await response.json();
}

testRollbackOrder();

----------C-----------
async function testConcurrentOrders() {
    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    )?.value;

    if (!token) {
        throw new Error("Anti-forgery token not found.");
    }

    async function createOrder(customerName) {
        const data = new URLSearchParams();

        data.append("__RequestVerificationToken", token);
        data.append("CustomerName", customerName);
        data.append("Items[0].ProductId", "4");
        data.append("Items[0].Quantity", "1");

        const response = await fetch("/Orders/Create", {
            method: "POST",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8"
            },
            body: data.toString()
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        return await response.json();
    }

    return await Promise.all([
        createOrder("Concurrent Test A"),
        createOrder("Concurrent Test B")
    ]);
}

testConcurrentOrders();