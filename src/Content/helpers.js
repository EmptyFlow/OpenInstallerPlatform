async function operationSequence(operations) {
    for (const operation of operations) {
        const { result } = await operation();
        if (!result) break;
    }
}

export {
    operationSequence
}