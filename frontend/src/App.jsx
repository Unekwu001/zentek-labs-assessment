import { useState } from "react";
import axios from "axios";
import "react-toastify/dist/ReactToastify.css";

const API = "https://localhost:7144/api/v1"; //please change this to your API URL if different

export default function App() {
    const [isLogin, setIsLogin] = useState(true);
    const [token, setToken] = useState(localStorage.getItem("token"));
    const [products, setProducts] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [searchColour, setSearchColour] = useState("");
    const [isAllView, setIsAllView] = useState(false);

    const [auth, setAuth] = useState({ email: "", password: "" });

    const [product, setProduct] = useState({
        name: "",
        description: "",
        price: "",
        stockQuantity: "",
        colour: "",
        base64String: "",
    });

    const [selectedFileName, setSelectedFileName] = useState("");

    // Image to base64
    const handleImageChange = (e) => {
        const file = e.target.files?.[0];
        if (!file) return;

        if (!file.type.startsWith("image/")) {
            alert("Please select a valid image file");
            return;
        }
        if (file.size > 5 * 1024 * 1024) {
            alert("Image size must be less than 5MB");
            return;
        }

        const reader = new FileReader();
        reader.onload = (event) => {
            setProduct((prev) => ({ ...prev, base64String: event.target.result }));
            setSelectedFileName(file.name);
        };
        reader.readAsDataURL(file);
    };

    // Fetch ALL products (paginated)
    const fetchAllProducts = async () => {
        try {
            const res = await axios.get(`${API}/Products/all?pageNumber=1&pageSize=10&ascending=true`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            const data = res.data.data || {};
            setProducts(data.items || []);
            setTotalCount(data.totalCount || 0);
            setIsAllView(true);
        } catch (err) {
            console.error(err);
            alert("Failed to load all products");
        }
    };

    // Fetch products by colour
    const fetchProductsByColour = async (colour) => {
        if (!colour?.trim()) return;
        try {
            const res = await axios.get(`${API}/Products/${colour.trim()}`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            setProducts(res.data.data || []);
            setTotalCount(res.data.data?.length || 0);
            setIsAllView(false);
        } catch (err) {
            console.error(err);
            alert("Failed to load products by colour");
        }
    };

    const handleAuth = async () => {
        try {
            const url = isLogin ? "/User/login" : "/User/register";
            const res = await axios.post(API + url, auth);
            const newToken = res.data.data?.token;
            if (newToken) {
                localStorage.setItem("token", newToken);
                setToken(newToken);
            }
            alert("Success!");
        } catch (err) {
            alert(err.response?.data?.message || "Error");
        }
    };

    const createProduct = async () => {
        if (!product.base64String) {
            alert("Please select a product image");
            return;
        }

        try {
            await axios.post(
                API + "/Products",
                {
                    name: product.name,
                    description: product.description,
                    price: Number(product.price) || 0,
                    stockQuantity: Number(product.stockQuantity) || 0,
                    colour: product.colour.trim(),
                    base64String: product.base64String,
                },
                { headers: { Authorization: `Bearer ${token}` } }
            );

            alert("Product created successfully!");

            // Reset form
            setProduct({
                name: "",
                description: "",
                price: "",
                stockQuantity: "",
                colour: "",
                base64String: "",
            });
            setSelectedFileName("");
        } catch (err) {
            alert(err.response?.data?.message || "Error creating product");
        }
    };

    return (
        <div className="container py-5">
            <h2 className="text-center mb-4">🚀 Product Dashboard</h2>

            {!token && (
                <div className="card p-4 mb-4">
                    <h4>{isLogin ? "Login" : "Register"}</h4>
                    <input className="form-control mb-2" placeholder="Email" onChange={(e) => setAuth({ ...auth, email: e.target.value })} />
                    <input type="password" className="form-control mb-2" placeholder="Password" onChange={(e) => setAuth({ ...auth, password: e.target.value })} />
                    <button className="btn btn-primary" onClick={handleAuth}>
                        {isLogin ? "Login" : "Register"}
                    </button>
                    <p className="mt-2 text-primary" style={{ cursor: "pointer" }} onClick={() => setIsLogin(!isLogin)}>
                        Switch to {isLogin ? "Register" : "Login"}
                    </p>
                </div>
            )}

            {token && (
                <>
                    <button className="btn btn-danger mb-3" onClick={() => { localStorage.removeItem("token"); setToken(null); }}>
                        Logout
                    </button>

                    <div className="row">
                        {/* Create Product Form */}
                        <div className="col-md-5">
                            <div className="card p-4">
                                <h5>Create New Product</h5>

                                <input className="form-control mb-2" placeholder="Product Name" value={product.name} onChange={(e) => setProduct({ ...product, name: e.target.value })} />
                                <input className="form-control mb-2" placeholder="Description" value={product.description} onChange={(e) => setProduct({ ...product, description: e.target.value })} />
                                <input className="form-control mb-2" placeholder="Price" type="number" value={product.price} onChange={(e) => setProduct({ ...product, price: e.target.value })} />
                                <input className="form-control mb-2" placeholder="Stock Quantity" type="number" value={product.stockQuantity} onChange={(e) => setProduct({ ...product, stockQuantity: e.target.value })} />
                                <input className="form-control mb-2" placeholder="Colour (e.g. Red)" value={product.colour} onChange={(e) => setProduct({ ...product, colour: e.target.value })} />

                                <div className="mb-3">
                                    <label className="form-label">Product Image</label>
                                    <input type="file" accept="image/*" className="form-control" onChange={handleImageChange} />
                                    {selectedFileName && <small className="text-success mt-1 d-block">Selected: {selectedFileName}</small>}
                                </div>

                                <button className="btn btn-success w-100" onClick={createProduct}>
                                    Create Product
                                </button>
                            </div>
                        </div>

                        {/* Products Display */}
                        <div className="col-md-7">
                            <div className="card p-4">
                                <div className="d-flex justify-content-between align-items-center mb-3">
                                    <h5>
                                        {isAllView ? "All Products" : "Products by Colour"}
                                        {totalCount > 0 && <span className="text-muted ms-2">({totalCount} total)</span>}
                                    </h5>
                                    <button className="btn btn-outline-success" onClick={fetchAllProducts}>
                                        View All Products
                                    </button>
                                </div>

                                {/* Search by Colour */}
                                <div className="d-flex mb-3">
                                    <input
                                        className="form-control me-2"
                                        placeholder="Enter colour (e.g. red)"
                                        value={searchColour}
                                        onChange={(e) => setSearchColour(e.target.value)}
                                    />
                                    <button className="btn btn-outline-primary" onClick={() => fetchProductsByColour(searchColour)}>
                                        Load by Colour
                                    </button>
                                </div>

                                {products.length === 0 && (
                                    <p className="text-muted text-center py-4">
                                        No products to display.<br />
                                        Click "View All Products" or search by colour.
                                    </p>
                                )}

                                <div className="row g-3">
                                    {products.map((p) => (
                                        <div key={p.id} className="col-12">
                                            <div className="card h-100">
                                                <div className="row g-0">
                                                    <div className="col-md-4">
                                                        <img
                                                            src={p.imageUrl || p.imageUr1}
                                                            alt={p.name}
                                                            className="img-fluid rounded-start"
                                                            style={{ height: "180px", objectFit: "cover", width: "100%" }}
                                                        />
                                                    </div>
                                                    <div className="col-md-8">
                                                        <div className="card-body">
                                                            <h5 className="card-title">{p.name}</h5>
                                                            <p className="card-text text-muted mb-1">{p.description}</p>
                                                            <p className="mb-1">
                                                                <strong>Colour:</strong> {p.colour} |
                                                                <strong> Price:</strong> ${p.price} {p.currencyCode}
                                                            </p>
                                                            <p className="mb-1">
                                                                <strong>Stock:</strong> {p.stockQuantity}
                                                            </p>
                                                            <small className="text-muted">ID: {p.id}</small>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
}