import {Bounce, ToastContainer} from "react-toastify";
import {FadeContainer} from "./sharedComponents/FadeContainer.tsx";
import {Outlet, ScrollRestoration} from "react-router-dom";
import 'react-toastify/dist/ReactToastify.css';
import ModalContainer from "./sharedComponents/ModalContainer.tsx";

function App() {

  return (
      <FadeContainer>
          <ScrollRestoration />
          <ModalContainer />
          <Outlet />
          <ToastContainer
              position="bottom-right"
              autoClose={3500}
              limit={1}
              hideProgressBar={false}
              newestOnTop
              closeOnClick
              rtl={false}
              pauseOnFocusLoss={false}
              draggable
              pauseOnHover={false}
              theme="colored"
              transition={Bounce}
          />
      </FadeContainer>
  )
}

export default App
