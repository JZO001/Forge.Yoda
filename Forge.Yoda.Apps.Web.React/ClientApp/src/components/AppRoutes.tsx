import { Counter } from "./Counter";
import { Home } from "./Home";
import Login from "./Login";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: "/login",
        element: <Login />
    },
    {
        path: '/counter',
        element: <Counter />
    }
];

export default AppRoutes;
