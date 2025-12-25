const express = require('express');
const cors = require('cors');
const sql = require('mssql');
const app = express();
app.use(cors());
app.use(express.json());

const dbConfig = {
    user: 'sa',
    password: '123', 
    server: 'localhost',
    database: 'CheburechnayaDB',
    options: { encrypt: false, trustServerCertificate: true }
};

const pool = new sql.ConnectionPool(dbConfig);
const poolConnect = pool.connect();

app.get('/api/suppliers', async (req, res) => {
    try {
        await poolConnect;
        const result = await pool.request().query('SELECT * FROM Suppliers');
        res.json(result.recordset);
    } catch (err) {
        res.json([]);
    }
});

app.get('/api/products', async (req, res) => {
    try {
        await poolConnect;
        const result = await pool.request().query('SELECT * FROM Products');
        res.json(result.recordset);
    } catch (err) {
        res.json([]);
    }
});

app.get('/api/employees', async (req, res) => {
    try {
        await poolConnect;
        const result = await pool.request().query('SELECT e.*, p.Title as PositionName FROM Employees e JOIN Positions p ON e.PositionId = p.Id');
        res.json(result.recordset);
    } catch (err) {
        res.json([]);
    }
});

app.get('/api/deliveries', async (req, res) => {
    try {
        await poolConnect;
        const result = await pool.request().query('SELECT * FROM Deliveries');
        res.json(result.recordset);
    } catch (err) {
        res.json([]);
    }
});

app.get('/api/orders', async (req, res) => {
    try {
        await poolConnect;
        const result = await pool.request().query('SELECT * FROM Orders');
        res.json(result.recordset);
    } catch (err) {
        res.json([]);
    }
});

app.post('/api/suppliers', async (req, res) => {
    try {
        const { companyName, contactPerson, phone } = req.body;
        await poolConnect;
        await pool.request()
            .input('name', sql.NVarChar, companyName)
            .input('contact', sql.NVarChar, contactPerson)
            .input('phone', sql.NVarChar, phone)
            .query('INSERT INTO Suppliers (CompanyName, ContactPerson, Phone) VALUES (@name, @contact, @phone)');
        res.json({ success: true });
    } catch (err) {
        res.json({ success: false });
    }
});

app.post('/api/products', async (req, res) => {
    try {
        const { productName, category, price, costPrice } = req.body;
        await poolConnect;
        await pool.request()
            .input('name', sql.NVarChar, productName)
            .input('cat', sql.NVarChar, category)
            .input('price', sql.Decimal, price)
            .input('cost', sql.Decimal, costPrice || null)
            .query('INSERT INTO Products (ProductName, Category, Price, CostPrice) VALUES (@name, @cat, @price, @cost)');
        res.json({ success: true });
    } catch (err) {
        res.json({ success: false });
    }
});

app.post('/api/employees', async (req, res) => {
    try {
        const { fullName, positionId, phoneNumber, hireDate } = req.body;
        await poolConnect;
        await pool.request()
            .input('name', sql.NVarChar, fullName)
            .input('pos', sql.Int, positionId)
            .input('phone', sql.NVarChar, phoneNumber)
            .input('date', sql.Date, hireDate)
            .query('INSERT INTO Employees (FullName, PositionId, PhoneNumber, HireDate) VALUES (@name, @pos, @phone, @date)');
        res.json({ success: true });
    } catch (err) {
        res.json({ success: false });
    }
});

app.post('/api/deliveries', async (req, res) => {
    try {
        const { supplierId, employeeId, driverName, driverPhone, vehicleNumber } = req.body;
        await poolConnect;
        await pool.request()
            .input('sup', sql.Int, supplierId)
            .input('emp', sql.Int, employeeId)
            .input('driver', sql.NVarChar, driverName)
            .input('dphone', sql.NVarChar, driverPhone)
            .input('car', sql.NVarChar, vehicleNumber)
            .query(`INSERT INTO Deliveries (SupplierId, EmployeeId, DriverName, DriverPhone, VehicleNumber, Status) 
              VALUES (@sup, @emp, @driver, @dphone, @car, 'Pending')`);
        res.json({ success: true });
    } catch (err) {
        res.json({ success: false });
    }
});

app.get('/api/positions', async (req, res) => {
    try {
        await poolConnect;
        const result = await pool.request().query('SELECT * FROM Positions');
        res.json(result.recordset);
    } catch (err) {
        res.json([]);
    }
});

app.put('/api/deliveries/:id/status', async (req, res) => {
    try {
        const { status } = req.body;
        await poolConnect;
        await pool.request()
            .input('id', sql.Int, req.params.id)
            .input('status', sql.NVarChar, status)
            .query('UPDATE Deliveries SET Status = @status WHERE Id = @id');
        res.json({ success: true });
    } catch (err) {
        res.json({ success: false });
    }
});

app.put('/api/orders/:id/status', async (req, res) => {
    try {
        const { status } = req.body;
        await poolConnect;
        await pool.request()
            .input('id', sql.Int, req.params.id)
            .input('status', sql.NVarChar, status)
            .query('UPDATE Orders SET Status = @status WHERE Id = @id');
        res.json({ success: true });
    } catch (err) {
        res.json({ success: false });
    }
});

app.get('/api/orders/:id', async (req, res) => {
    try {
        await poolConnect;
        const result = await pool.request()
            .input('id', sql.Int, req.params.id)
            .query('SELECT * FROM Orders WHERE Id = @id');
        res.json(result.recordset[0] || {});
    } catch (err) {
        res.json({});
    }
});

app.get('/api/orders/:id/items', async (req, res) => {
    try {
        await poolConnect;
        const result = await pool.request()
            .input('id', sql.Int, req.params.id)
            .query('SELECT * FROM OrderItems WHERE OrderId = @id');
        res.json(result.recordset);
    } catch (err) {
        res.json([]);
    }
});

app.listen(5023, () => console.log('API работает на порту 5023'));