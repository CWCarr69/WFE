import React from 'react';
import { ClipLoader } from 'react-spinners';


class SpinnerComponent extends React.Component {
    state = {
        loading: true
    };
	render() {
		return (
			<div className='sweet-loading'>
				<ClipLoader
					className="clip-loader"
					sizeUnit={"px"}
					size={120}
					color={'#000'}
					loading={this.state.loading}
				/>
			</div> 
		)
	}
}

export default SpinnerComponent;