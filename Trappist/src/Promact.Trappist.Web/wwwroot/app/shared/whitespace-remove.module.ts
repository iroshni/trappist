﻿import { Directive, forwardRef } from '@angular/core';
import { FormControl, NG_VALIDATORS } from '@angular/forms';

    /**
    * function used to remove whitespaces
    */
export function removeWhiteSpace() {
    return (c: FormControl) => {
        if (c.value != null && c.value!=undefined && c.value!='') {
            var toRemoveWhiteSpace = c.value;
            let whitespace_REGEX = toRemoveWhiteSpace.replace(/^\s+|\s+$|\s+(?=\s)/g, "");
            if (whitespace_REGEX.)
            {
                
                }
                return false;
            return true;
        }
    };
}

@Directive({
    selector: '[whitespaceremove]',
    providers: [
        { provide: NG_VALIDATORS, useClass: RemoveWhiteSpace, multi: true }
    ]
})
export class RemoveWhiteSpace {

    whiteSpaceValidator: Function;

    constructor() {
        this.whiteSpaceValidator = removeWhiteSpace();
    }
    validate(c: FormControl) {
        return this.whiteSpaceValidator(c);
    }
}