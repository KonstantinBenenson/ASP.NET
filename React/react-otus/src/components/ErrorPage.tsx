export const ErrorPage = (props) => {
  const { error } = props;
  return (
    <div
      style={{
        display: "flex",
        justifyContent: "left",
        flexDirection: "column",
        border: "1px solid red",
        width: "100%",
        padding: "20px 20px",
      }}
    >
      <p style={{ textAlign: "left" }}>{error}</p>
      <button onClick={() => window.location.reload()} style={{ width: "30%" }}>
        Return
      </button>
    </div>
  );
};
