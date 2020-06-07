import React from "react";
import LinearProgress from "@material-ui/core/LinearProgress/LinearProgress";

const backStyles = {
    backgroundColor: "rgba(0, 0, 0, 0.2)",
    width: '100%',
    height: '100%',
    position: "fixed",
    top: 0,
    left: 0
};

const progressStyles = {
    position: 'fixed',
    width: '80%',
    marginLeft: '10%',
    bottom: '10%'
};

export default function GlobalLoader() {
    return (
        <div>
            <div style={backStyles}>
            </div>
            <LinearProgress style={progressStyles} variant={'query'}/>
        </div>
    )
}
