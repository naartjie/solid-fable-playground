import {Component} from 'solid-js'

type Props = {
    firstName: string
    lastName: string
}

export const MyComponent: Component<Props> = (props) => {
    return <>
        <div>Hi {props.firstName}</div>
        <div>Bye {props.lastName}</div>
    </>
}
