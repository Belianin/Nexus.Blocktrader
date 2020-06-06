import React from "react";
import logo from "../logo.png";

export default function Header() {
    return (
        <header>
            <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" />
            <img src={logo} style={{height: 128, width: "auto"}} alt="logo"/>
        </header>
    )
}
